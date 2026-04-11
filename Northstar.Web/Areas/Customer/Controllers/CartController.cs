using Northstar.DataAccess.IRepository;
using Northstar.Models;
using Northstar.Models.ViewModels;
using Northstar.DataAccess;
using Northstar.Utility;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using Northstar.Web.Controllers;

namespace Northstar.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController(IUnitOfWork unitOfWork) : RepositoryBoundController<ShoppingCartItem, IShoppingCartItemRepository>(unitOfWork), IUnitOfWorkProvider
    {
        protected override IShoppingCartItemRepository Repo => unitOfWork.ShoppingCarts;
        protected override string DefaultFeedbackName => "Shopping Cart";
        protected override string? DefaultIncludeProperties => "Product";

        IUnitOfWork IUnitOfWorkProvider.UnitOfWork => unitOfWork;

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
            var orderHeader = unitOfWork.OrderHeaderRepository.GetById(id, track: false);
            var paymentSuccessful = true;

            //If payment is not delayed, it is validated and fields are attributed
            if(orderHeader.OrderStatus != SD.PAYMENT_STATUS_DELAYED)
            {
                paymentSuccessful = StripeUtility.ValidatePayment(
                    orderHeader.SessionId ?? string.Empty, 
                    paymentIntentId => AssignPaymentValues(orderHeader, paymentIntentId));
            }

            if(!paymentSuccessful)
            {
                return View(id);
            }    

            orderHeader.OrderDate = DateTime.Now;
            unitOfWork.OrderHeaderRepository.Update(orderHeader);
            
            MoveCartItemsToOrder(orderHeader.ApplicationUserId, orderHeader.Id);
            unitOfWork.Save();
            
            return View(id);
        }

        public IActionResult OrderFailure(int id)
        {
            unitOfWork.OrderHeaderRepository.RemoveById(id);
            return RedirectToAction(nameof(Summary));
        }

        [HttpPost]
        public IActionResult Summary(ShoppingCartVM vm)
        {
            // Load the user separately because the navigation property is not posted back with the form.
            var appUser = unitOfWork.ApplicationUserRepository.GetFirst(
                u => u.Id == vm.OrderHeader.ApplicationUserId,
                track: true);
                
            //If user requested to set order address as account defaults, set them here
            if(vm.SetAddressAsDefault)
            {
                var container = appUser as IAddressContainer;
                container.FetchDetails(vm.OrderHeader);
                unitOfWork.Save();
            }

            // Company accounts are approved for delayed payment; other users must complete checkout first.
            var isCompanyUser = appUser.CompanyId.GetValueOrDefault() > 0;

            if(isCompanyUser)
            {
                unitOfWork.OrderHeaderRepository.Add(vm.OrderHeader);
                unitOfWork.Save();
                return RedirectToAction(nameof(OrderConfirmation), vm.OrderHeader.Id);
            }

            vm.OrderHeader.PaymentStatus = SD.PAYMENT_STATUS_DELAYED;
            vm.OrderHeader.OrderStatus = SD.ORDER_STATUS_APPROVED;
            unitOfWork.OrderHeaderRepository.Add(vm.OrderHeader);
            unitOfWork.Save();

            var productList = Repo.GetAll(
                e => e.ApplicationUser.Id == vm.OrderHeader.ApplicationUserId, 
                includeProperties: DefaultIncludeProperties);

            return StripeUtility.PromptStripePayment(unitOfWork, Response, new StripeProcessDto
            {
                headerId = vm.OrderHeader.Id,
                items = productList,
                area = "Customer",
                page = "Cart",
                sucessAction = "OrderConfirmation",
                failAction = "OrderFailure",
                sucessUsesId = true,
            });
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

        private ShoppingCartVM BuildViewModel()
        {
            if(!User.TryGetId(out var userId))
            {
                return new();
            }

            var appUser = unitOfWork.ApplicationUserRepository.GetFirst(u => u.Id == userId);
            var cartItems = Repo.GetAll(
                e => e.ApplicationUser.Id == userId, 
                includeProperties: DefaultIncludeProperties);

            var header = new OrderHeader
            {
                ApplicationUserId = userId,
                OrderTotal = cartItems.Sum(e => e.TotalCost),

                //Add default init values for order and payment status
                OrderStatus = SD.ORDER_STATUS_PENDING,
                PaymentStatus = SD.PAYMENT_STATUS_PENDING,
            };

            var asContainer = header as INamedAddressContainer;
            asContainer.FetchDetails(appUser);

            foreach (var item in cartItems)
            {
                var image = unitOfWork.ProductImagesRepository.GetFirst(i => i.ProductId.Equals(item.Product.Id));
                item.Product.Images = [image];
            }

            return new ShoppingCartVM
            {
                ProductList = cartItems,
                OrderHeader = header
            };
        }
      
        private void AssignPaymentValues(OrderHeader orderHeader, string paymentIntentId)
        {
            orderHeader.PaymentDate = DateTime.Now;
            orderHeader.PaymentIntentId = paymentIntentId;
            orderHeader.PaymentStatus = SD.PAYMENT_STATUS_APPROVED;
            orderHeader.OrderStatus = SD.ORDER_STATUS_APPROVED;
        }

        private void MoveCartItemsToOrder(string userId, int orderHeaderId)
        {
            //Fetches cart items based on userId
            var cartItems = Repo.GetAll(
                e => e.ApplicationUserId.Equals(userId), 
                includeProperties: DefaultIncludeProperties);

            //Converts into Order Items
            var orderItems = cartItems.Select(item =>
            {
                var orderItem = OrderItemDetails.BuildFrom(item);
                orderItem.OrderHeaderId = orderHeaderId;
                return orderItem;
            });

            //Order items are added, cart is emptied
            unitOfWork.OrderItemDetailsRepository.AddRange([.. orderItems]);
            Repo.RemoveRange([.. cartItems]);
            HttpContext.Session.SetInt32(SD.CART_SESSION, 0);
        }

    }
}
