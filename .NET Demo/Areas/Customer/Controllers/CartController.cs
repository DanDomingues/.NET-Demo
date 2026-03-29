using Demo.DataAccess.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.DataAccess;
using Demo.Utility;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using ASP.NET_Debut.Controllers;

namespace ASP.NET_Debut.Areas.Customer.Controllers
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

                //Details fetching, can be overwritten later on
                Name = appUser.Name ?? string.Empty,
                PhoneNumber = appUser.PhoneNumber ?? string.Empty,
                StreetAddress = appUser.StreetAddress ?? string.Empty,
                City = appUser.City ?? string.Empty,
                State = appUser.State ?? string.Empty,
                PostalCode = appUser.PostalCode ?? string.Empty,
            };

            //NOTE: By adding a header to the DB, the id gets automatically assigned
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
            //View model received back from html cannot retain objects and structs, so re-fetching products and the user is necessary
            //Luckly, all the editable properties are strings that come filled in the vm
            vm.ProductList = Repo.GetAll(
                e => e.ApplicationUser.Id == vm.OrderHeader.ApplicationUserId, 
                includeProperties: DefaultIncludeProperties);

            //Header.ApplicationUser is bound by the matching KF, so we can't submit it with a value
            //Alternatively, we can fetch and store the user in a local field and use it while we haven't added this header to it's repo yet
            var appUser = unitOfWork.ApplicationUserRepository
                .GetFirstOrDefault(u => u.Id == vm.OrderHeader.ApplicationUserId);

            //Order total needs to be recalculated as the products may have been changed in the view
            vm.OrderHeader.OrderTotal = vm.ProductList.Sum(item => item.TotalCost);

            //For companies, we want to pre-approve the payment and proceed with the order, for users, payment preceeds order approval
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
