using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    [Area("Customer")]
    public class CartController(IUnitOfWork unitOfWork) : RepositoryBoundController<ShoppingCartItem, IShoppingCartItemRepository>(unitOfWork)
    {
        protected override string DefaultFeedbackName => "Shopping Cart";
        protected override IShoppingCartItemRepository Repo => unitOfWork.ShoppingCarts;

        //Possibly move this up a layer or two
        protected string GetUserId()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            return claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        private ShoppingCartVM BuildViewModel()
        {
            var userId = GetUserId();
            var appUser = unitOfWork.ApplicationUserRepository.GetFirstOrDefault(u => u.Id == userId);
            var orderItems = Repo.GetAll(e => e.ApplicationUser.Id == userId, includeProperties: "Product");

            var header = new OrderHeader
            {
                ApplicationUserId = userId,
                OrderTotal = orderItems.Select(e => e.TotalCost).Sum(),

                //Details fetching, can be overwritten later on
                Name = appUser.Name,
                PhoneNumber = appUser.PhoneNumber ?? string.Empty,
                StreetAddress = appUser.StreetAddress ?? string.Empty,
                City = appUser.City ?? string.Empty,
                State = appUser.State ?? string.Empty,
                PostalCode = appUser.PostalCode ?? string.Empty,
            };

            return new ShoppingCartVM
            {
                ProductList = orderItems,
                OrderHeader = header
            };
        }

        public override IActionResult Index()
        {
            return View(BuildViewModel());
        }

        public IActionResult Summary()
        {
            return View(BuildViewModel());
        }

        [HttpPost, ActionName("Summary")]
        public IActionResult SummaryPost(ShoppingCartVM vm)
        {
            //Order total needs to be recalculated as the products may have been changed in the view
            vm.OrderHeader.OrderTotal = vm.ProductList.Select(item => item.TotalCost).Sum();

            //For companies, we want to pre-approve the payment and proceed with the order, for users, payment preceeds order approval
            var isCompanyUser = vm.OrderHeader.ApplicationUser.CompanyId.GetValueOrDefault() > 0;
            vm.OrderHeader.PaymentStatus = isCompanyUser ? SD.PAYMENT_STATUS_DELAYED : SD.PAYMENT_STATUS_PENDING;
            vm.OrderHeader.OrderStatus = isCompanyUser ? SD.ORDER_STATUS_APPROVED : SD.ORDER_STATUS_PENDING;

            unitOfWork.OrderHeaderRepository.Add(vm.OrderHeader);

            foreach (var item in vm.ProductList)
            {
                unitOfWork.OrderItemDetailsRepository.Add(new()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = vm.OrderHeader.Id,
                    Count = item.Count,
                    Price = item.TotalCost,
                });   
            }

            if(!isCompanyUser)
            {
                return PromptStripePayment(vm);
            }

            unitOfWork.Save();

            return RedirectToAction(nameof(OrderConfirmation), vm.OrderHeader.Id);
        }

        private IActionResult PromptStripePayment(ShoppingCartVM vm)
        {
            var options = BuildStripeSessionOptions(vm);
            var service = new SessionService();
            var session = service.Create(options);
            
            Response.Headers.Add("Location", session.Url);

            unitOfWork.OrderHeaderRepository.UpdatePaymentID(vm.OrderHeader.Id, session.Id, session.PaymentIntentId);
            unitOfWork.Save();
            
            return new StatusCodeResult(303);
        }

        private SessionCreateOptions BuildStripeSessionOptions(ShoppingCartVM vm, string domain = "https://localhost:?/")
        {
            return new SessionCreateOptions
            {
                SuccessUrl = $"{domain}customer/cart/OrderConfirmation?id={vm.OrderHeader.Id}",
                CancelUrl = $"{domain}customer/cart/index",
                LineItems = [.. vm.ProductList.Select(item =>
                    {
                        return new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.TotalCost * 100),
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.Title,
                                }
                            },
                            Quantity = item.Count,
                        };
                    })],
                Mode = "payment",
            };
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }

        public IActionResult Add(int id)
        {
            Repo.GetById(id).Count++;
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Reduce(int id)
        {
            var obj = Repo.GetById(id);

            if (obj.Count <= 1)
            {
                return Remove(id);
            }

            obj.Count--;
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            Repo.Remove(Repo.GetById(id));
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
