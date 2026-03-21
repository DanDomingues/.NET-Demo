using Demo.DataAccess.Repository;
using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    [Authorize]
    public class OrderController(IUnitOfWork unitOfWork) : RepositoryBoundController<OrderHeader, IOrderHeaderRepository>(unitOfWork)
    {
        protected override IOrderHeaderRepository Repo => unitOfWork.OrderHeaderRepository;

        protected override string DefaultFeedbackName => "Order";

        protected override string? DefaultIncludeProperties => "ApplicationUser";

        public IActionResult Details(int? orderId)
        {
            var header = Repo.GetById(orderId, includeProperties: DefaultIncludeProperties);
            var orderItems = unitOfWork.OrderItemDetailsRepository.GetAll(
                details => details.OrderHeaderId == header.Id, 
                track: false, 
                includeProperties: "Product");
            var vm = new OrderVM { Header = header, Details = orderItems };
            return View(vm);
        }

        [HttpPost, Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
        public IActionResult UpdateDetails(OrderVM vm)
        {
            var orderHeader = Repo.GetFirstOrDefault(order => order.ApplicationUserId.Equals(vm.Header.ApplicationUserId), track: false);

            // Map fields from vm.Header to orderHeader
            orderHeader.Name = vm.Header.Name;
            orderHeader.PhoneNumber = vm.Header.PhoneNumber;
            orderHeader.StreetAddress = vm.Header.StreetAddress;
            orderHeader.City = vm.Header.City;
            orderHeader.State = vm.Header.State;
            orderHeader.PostalCode = vm.Header.PostalCode;           
            orderHeader.TrackingNumber = vm.Header.TrackingNumber ?? orderHeader.TrackingNumber;
            orderHeader.Carrier = vm.Header.Carrier ?? orderHeader.Carrier;
                
            return UpdateRepo(
                orderHeader, 
                unitOfWork.OrderHeaderRepository.Update, 
                feedback: "Order Updated Sucessfully", 
                redirection: "Details", 
                redirectionArgs: new { orderId = orderHeader.Id });
        }

        [HttpPost, Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
        public IActionResult StartProcess(OrderVM vm)
        {
            Repo.UpdateOrderStatus(vm.Header.Id, SD.ORDER_STATUS_PROCESSING);
            AddOperationFeedback("Order Details Changed Sucessfully");
            unitOfWork.Save();         

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
        public IActionResult ShipOrder(OrderVM vm)
        {
            var header = Repo.GetById(vm.Header.Id);

            header.TrackingNumber = vm.Header.TrackingNumber;
            header.Carrier = vm.Header.Carrier;
            header.OrderStatus = SD.ORDER_STATUS_SHIPPED;
            header.ShippingDate = DateTime.Now;

            if(header.OrderStatus.Equals(SD.PAYMENT_STATUS_DELAYED))
            {
                header.PaymentDate = DateTime.Now.AddDays(30);
            }

            return UpdateRepo(
                header,
                Repo.Update,
                feedback: "Order Shipped",
                redirection: "Order set to Ship",
                redirectionArgs: new { orderId = header.Id });
        }

        [HttpPost, Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
        public IActionResult CancelOrder(OrderVM vm)
        {
            var header = Repo.GetById(vm.Header.Id);

            if(header.PaymentStatus.Equals(SD.PAYMENT_STATUS_APPROVED))
            {
                var service = new RefundService();
                service.Create(new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = header.PaymentIntentId,
                });
                Repo.UpdateOrderStatus(header.Id, SD.ORDER_STATUS_CANCELLED);
                Repo.UpdatePaymentStatus(header.Id, SD.PAYMENT_STATUS_REFUNDED);
            }
            else
            {
                Repo.UpdateOrderStatus(header.Id, SD.ORDER_STATUS_CANCELLED);
                Repo.UpdatePaymentStatus(header.Id, SD.PAYMENT_STATUS_REJECTED);
            }
            
            return RedirectToAction(nameof(Details), new { orderId = header.Id });
        }

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            var userId = User.GetUserId();
            var all = User.HasAdminRights() ?
                Repo.GetAll(includeProperties: DefaultIncludeProperties,track: false) :
                Repo.GetAll(header => header.ApplicationUserId == userId, includeProperties: DefaultIncludeProperties, track: false);

            Func<OrderHeader, bool> filter = status switch
            {
                "paymentpending" => header => header.PaymentStatus == SD.PAYMENT_STATUS_PENDING,
                "inprocess" => header => header.OrderStatus == SD.ORDER_STATUS_PROCESSING,
                "completed" => header => header.OrderStatus == SD.PAYMENT_STATUS_APPROVED,
                "approved" => header => header.OrderStatus == SD.ORDER_STATUS_APPROVED,
                _ => header => true,
            };

            return Json(data: all.Where(filter));
        }
    }
}
