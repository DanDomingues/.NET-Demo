using ASP.NET_Debut.Areas.Admin.Controllers;
using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace ASP.NET_Debut.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController(IUnitOfWork unitOfWork) : RepositoryBoundController<ShoppingCartItem, IShoppingCartItemRepository>(unitOfWork), IUnitOfWorkProvider
    {
        protected override string DefaultFeedbackName => "Shopping Cart";
        protected override IShoppingCartItemRepository Repo => unitOfWork.ShoppingCarts;

        IUnitOfWork IUnitOfWorkProvider.UnitOfWork => unitOfWork;

        private ShoppingCartVM BuildViewModel()
        {
            var userId = User.GetUserId();
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
            //View model received back from html cannot retain objects and structs, so re-fetching products and the user is necessary
            //Luckly, all the editable properties are strings that come filled in the vm
            vm.ProductList = Repo.GetAll(e => e.ApplicationUser.Id == vm.OrderHeader.ApplicationUserId, includeProperties: "Product");

            //Header.ApplicationUser is bound by the matching KF, so we can't submit it with a value
            //Alternatively, we can fetch and store the user in a local field and use it while we haven't added this header to it's repo yet
            var appUser = unitOfWork.ApplicationUserRepository.GetFirstOrDefault(u => u.Id == vm.OrderHeader.ApplicationUserId);

            //Order total needs to be recalculated as the products may have been changed in the view
            vm.OrderHeader.OrderTotal = vm.ProductList.Select(item => item.TotalCost).Sum();

            //For companies, we want to pre-approve the payment and proceed with the order, for users, payment preceeds order approval
            var isCompanyUser = appUser.CompanyId.GetValueOrDefault() > 0;
            vm.OrderHeader.PaymentStatus = isCompanyUser ? SD.PAYMENT_STATUS_DELAYED : SD.PAYMENT_STATUS_PENDING;
            vm.OrderHeader.OrderStatus = isCompanyUser ? SD.ORDER_STATUS_APPROVED : SD.ORDER_STATUS_PENDING;

            unitOfWork.OrderHeaderRepository.Add(vm.OrderHeader);

            if(!isCompanyUser)
            {
                return StripeUtility.PromptStripePayment(unitOfWork, Response, new StripeProcessDto
                {
                    items = vm.ProductList,
                    headerId = vm.OrderHeader.Id,
                    area = "Admin",
                    page = "cart",
                    sucessAction = "OrderConfirmation",
                    failAction = "Index",
                    sucessUsesId = true,
                });
            }

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

            unitOfWork.Save();

            return RedirectToAction(nameof(OrderConfirmation), vm.OrderHeader.Id);
        }

        public IActionResult OrderConfirmation(int id)
        {
            var paymentSuccessful = true;
            var orderHeader = unitOfWork.OrderHeaderRepository.GetById(id, includeProperties: "ApplicationUser");

            if(orderHeader.OrderStatus != SD.PAYMENT_STATUS_DELAYED)
            {
                paymentSuccessful = false;
                var service = new SessionService();
                var session = service.Get(orderHeader.SessionId);
                
                if(session?.PaymentStatus?.ToLower() == "paid")
                {
                    unitOfWork.OrderHeaderRepository.UpdatePaymentID(id, session.PaymentIntentId);
                    unitOfWork.OrderHeaderRepository.UpdatePaymentStatus(id, SD.PAYMENT_STATUS_APPROVED);
                    unitOfWork.OrderHeaderRepository.UpdateOrderStatus(id, SD.ORDER_STATUS_APPROVED);
                    unitOfWork.Save();
                }
            }

            if(paymentSuccessful)
            {
                var cart = Repo.GetAll(e => e.ApplicationUserId == orderHeader.ApplicationUserId);
                Repo.RemoveRange([.. cart]);
                HttpContext.Session.SetInt32(SD.CART_SESSION, 0);
            }

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
            this.RefreshCartItemsCount();
            return RedirectToAction(nameof(Index));
        }
    }
}
