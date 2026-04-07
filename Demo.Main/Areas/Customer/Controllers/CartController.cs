using Demo.DataAccess.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.DataAccess;
using Demo.Utility;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using Demo.Main.Controllers;

namespace Demo.Main.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController(IUnitOfWork unitOfWork) : RepositoryBoundController<ShoppingCartItem, IShoppingCartItemRepository>(unitOfWork), IUnitOfWorkProvider
    {
        protected override IShoppingCartItemRepository Repo => unitOfWork.ShoppingCarts;
        protected override string DefaultFeedbackName => "Shopping Cart";
        protected override string? DefaultIncludeProperties => "Product";

        IUnitOfWork IUnitOfWorkProvider.UnitOfWork => unitOfWork;

        private ShoppingCartVM BuildViewModel()
        {
            if(!User.TryGetId(out var userId))
            {
                return new();
            }

            var appUser = unitOfWork.ApplicationUserRepository.GetFirstOrDefault(u => u.Id == userId);
            var cartItems = Repo.GetAll(e => e.ApplicationUser.Id == userId, includeProperties: DefaultIncludeProperties);

            var header = new OrderHeader
            {
                ApplicationUserId = userId,
                OrderTotal = cartItems.Sum(e => e.TotalCost),

                // Pre-fill checkout fields from the current user profile.
                FirstName = appUser.FirstName ?? string.Empty,
                LastName = appUser.LastName ?? string.Empty,
                PhoneNumber = appUser.PhoneNumber ?? string.Empty,
                StreetAddress = appUser.StreetAddress ?? string.Empty,
                City = appUser.City ?? string.Empty,
                State = appUser.State ?? string.Empty,
                PostalCode = appUser.PostalCode ?? string.Empty,
            };

            // Save early so the order header has an ID before checkout continues.
            unitOfWork.OrderHeaderRepository.Add(header);
            unitOfWork.Save();

            foreach (var item in cartItems)
            {
                var image = unitOfWork.ProductImagesRepository.GetFirstOrDefault(i => i.ProductId.Equals(item.Product.Id));
                item.Product.Images = [image];
            }

            return new ShoppingCartVM
            {
                ProductList = cartItems,
                OrderHeader = header
            };
        }

        public IActionResult Index()
        {
            if(!User.TryGetId(out var _))
            {
                return this.RedirectToLogin();
            }
            return View(BuildViewModel());
        }

        public IActionResult Summary()
        {
            return View(BuildViewModel());
        }

        public IActionResult OrderConfirmation(int id)
        {
            var paymentSuccessful = true;
            var orderHeader = unitOfWork.OrderHeaderRepository.GetById(
                id, 
                includeProperties: "ApplicationUser");
            
            orderHeader.OrderDate = DateTime.Now;

            if(orderHeader.OrderStatus != SD.PAYMENT_STATUS_DELAYED)
            {
                paymentSuccessful = false;
                var service = new SessionService();
                var session = service.Get(orderHeader.SessionId);
                
                if(session?.PaymentStatus?.ToLower() == "paid")
                {
                    paymentSuccessful = true;
                    unitOfWork.OrderHeaderRepository.UpdatePaymentID(id, session.PaymentIntentId);
                    unitOfWork.OrderHeaderRepository.UpdatePaymentStatus(id, SD.PAYMENT_STATUS_APPROVED);
                    unitOfWork.OrderHeaderRepository.UpdateOrderStatus(id, SD.ORDER_STATUS_APPROVED);
                }
            }

            if(paymentSuccessful)
            {
                orderHeader.PaymentDate = DateTime.Now;

                var cartItems = Repo.GetAll(
                    e => e.ApplicationUserId == orderHeader.ApplicationUserId, 
                    includeProperties: DefaultIncludeProperties).ToArray();

                var orderItems = cartItems
                    .Select(item => new OrderItemDetails
                    {
                        ProductId = item.ProductId,
                        OrderHeaderId = id,
                        Count = item.Count,
                        Price = item.Product.Price
                    })
                    .ToArray();

                unitOfWork.OrderItemDetailsRepository.AddRange(orderItems);
                Repo.RemoveRange([.. cartItems]);
                HttpContext.Session.SetInt32(SD.CART_SESSION, 0);
            }
            
            unitOfWork.Save();

            return View(id);
        }

        [HttpPost]
        public IActionResult Summary(ShoppingCartVM vm)
        {
            // Load the user separately because the navigation property is not posted back with the form.
            var appUser = unitOfWork.ApplicationUserRepository.GetFirstOrDefault(
                u => u.Id == vm.OrderHeader.ApplicationUserId,
                track: true);

            //If user requested to set order address as account defaults, set them here
            if(vm.SetAddressAsDefault)
            {
                appUser.PhoneNumber = vm.OrderHeader.PhoneNumber ?? appUser.PhoneNumber;
                appUser.StreetAddress = vm.OrderHeader.StreetAddress ?? appUser.StreetAddress;
                appUser.City = vm.OrderHeader.City ?? appUser.City;
                appUser.State = vm.OrderHeader.State ?? appUser.State;
                appUser.PostalCode = vm.OrderHeader.PostalCode ?? appUser.PostalCode;
            }

            // Rehydrate related data that is not preserved by form posting.
            vm.ProductList = Repo.GetAll(
                e => e.ApplicationUser.Id == vm.OrderHeader.ApplicationUserId, 
                includeProperties: DefaultIncludeProperties);


            // Recalculate totals from the current product data.
            vm.OrderHeader.OrderTotal = vm.ProductList.Sum(item => item.TotalCost);

            // Company accounts are approved for delayed payment; other users must complete checkout first.
            var isCompanyUser = appUser.CompanyId.GetValueOrDefault() > 0;
            vm.OrderHeader.PaymentStatus = isCompanyUser ? SD.PAYMENT_STATUS_DELAYED : SD.PAYMENT_STATUS_PENDING;
            vm.OrderHeader.OrderStatus = isCompanyUser ? SD.ORDER_STATUS_APPROVED : SD.ORDER_STATUS_PENDING;

            if(!isCompanyUser)
            {
                return StripeUtility.PromptStripePayment(unitOfWork, Response, new StripeProcessDto
                {
                    items = vm.ProductList,
                    headerId = vm.OrderHeader.Id,
                    area = "Customer",
                    page = "cart",
                    sucessAction = "OrderConfirmation",
                    failAction = "Index",
                    sucessUsesId = true,
                });
            }

            unitOfWork.Save();

            return RedirectToAction(nameof(OrderConfirmation), vm.OrderHeader.Id);
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
            this.RefreshCartItemsCount();
            return RedirectToAction(nameof(Index));
        }
    }
}
