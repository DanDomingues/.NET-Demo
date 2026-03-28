using Demo.DataAccess.Repository;
using Demo.DataAccess.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.DataAccess;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using ASP.NET_Debut.Controllers;

namespace ASP.NET_Debut.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController(IUnitOfWork unitOfWork) : RepositoryBoundController<OrderHeader, IOrderHeaderRepository>(unitOfWork)
    {
        protected override IOrderHeaderRepository Repo => unitOfWork.OrderHeaderRepository;
        protected override string DefaultFeedbackName => "Order";
        protected override string? DefaultIncludeProperties => "ApplicationUser";

        public IActionResult Details(int? id)
        {
            var header = Repo.GetById(id, includeProperties: DefaultIncludeProperties);
            var orderItems = unitOfWork.OrderItemDetailsRepository.GetAll(
                details => details.OrderHeaderId == header.Id, 
                track: false, 
                includeProperties: "Product");

            return View(new OrderVM 
            { 
                Header = header, 
                Details = orderItems 
            });
        }

        [HttpPost, Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
        public IActionResult UpdateDetails(OrderVM vm)
        {
            var orderHeader = Repo.GetFirstOrDefault(order => order.Id.Equals(vm.Header.Id), track: false);
               
            // Map fields from vm.Header to orderHeader
            orderHeader.Name = vm.Header.Name ?? orderHeader.Name;
            orderHeader.PhoneNumber = vm.Header.PhoneNumber ?? orderHeader.PhoneNumber;
            orderHeader.StreetAddress = vm.Header.StreetAddress ?? orderHeader.StreetAddress;
            orderHeader.City = vm.Header.City ?? orderHeader.City;
            orderHeader.State = vm.Header.State ?? orderHeader.State;
            orderHeader.PostalCode = vm.Header.PostalCode ?? orderHeader.PostalCode;           
            orderHeader.TrackingNumber = vm.Header.TrackingNumber ?? orderHeader.TrackingNumber;
            orderHeader.Carrier = vm.Header.Carrier ?? orderHeader.Carrier;
                
            return UpdateRepo(
                orderHeader, 
                unitOfWork.OrderHeaderRepository.Update, 
                feedback: "Order Updated Sucessfully", 
                redirection: "Index");
        }

        [HttpPost, Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
        public IActionResult StartProcess(OrderVM vm)
        {
            var header = Repo.GetById(vm.Header.Id, track: true);
            
            //TODO-1: Work carrier options into a dropdown input
            var defaultCarrier = "iCarry";
            header.Carrier = defaultCarrier;
            header.OrderStatus = SD.ORDER_STATUS_PROCESSING;
            header.TrackingNumber = Guid.NewGuid().ToString();
            this.AddOperationFeedback("Order Details Changed Sucessfully");
            unitOfWork.Save();         

            return UpdateRepo(
                header,
                Repo.Update,
                feedback: "Order Shipped",
                redirection: "Details",
                redirectionArgs: new { id = header.Id });
        }

        [HttpPost, Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
        public IActionResult ShipOrder(OrderVM vm)
        {
            var header = Repo.GetById(vm.Header.Id, track: false);

            header.TrackingNumber = vm.Header.TrackingNumber ?? header.TrackingNumber;
            header.Carrier = vm.Header.Carrier ?? header.Carrier;
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
                redirection: "Details",
                redirectionArgs: new { id = header.Id });
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
        
        [HttpPost, Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
        public IActionResult RequestPayment(OrderVM vm)
        {
            vm.Header = Repo
                .GetById(vm.Header.Id, includeProperties: DefaultIncludeProperties);
            vm.Details = unitOfWork.OrderItemDetailsRepository
                .GetAll(item => item.OrderHeaderId.Equals(vm.Header.Id), includeProperties: "Product");
            
            return StripeUtility.PromptStripePayment(unitOfWork, Response, new()
            {
               items = vm.Details,
               area = "Customer",
               page = "order",
               sucessAction = "PaymentConfirmation",
               failAction = "Details",
               sucessUsesId = true,
               failUsesId = true 
            });
        }

        [HttpGet]
        public IActionResult GetAllByStatus(string status)
        {
            if(!User.TryGetId(out var userId))
            {
                return Json(data: Array.Empty<OrderHeader>());
            }

            var all = User.HasAdminRights() ?
                Repo.GetAll(
                    includeProperties: DefaultIncludeProperties, 
                    track: false) :
                Repo.GetAll(
                    header => header.ApplicationUserId == userId, 
                    includeProperties: DefaultIncludeProperties, 
                    track: false);

            Func<OrderHeader, bool> filter = status switch
            {
                "paymentpending" => header => header.PaymentStatus == SD.PAYMENT_STATUS_PENDING,
                "inprocess" => header => header.OrderStatus == SD.ORDER_STATUS_PROCESSING,
                "completed" => header => header.OrderStatus == SD.PAYMENT_STATUS_APPROVED,
                "approved" => header => header.OrderStatus == SD.ORDER_STATUS_APPROVED,
                _ => header => !string.IsNullOrEmpty(header.OrderStatus),
            };

            return Json(new { data = all.Where(filter) });
        }

        public IActionResult PaymentConfirmation(int id)
        {
            var orderHeader = unitOfWork.OrderHeaderRepository.GetById(id);
            if(orderHeader.OrderStatus == SD.PAYMENT_STATUS_DELAYED)
            {
                var service = new SessionService();
                var session = service.Get(orderHeader.SessionId);
                
                if(session?.PaymentStatus?.ToLower() == "paid")
                {
                    unitOfWork.OrderHeaderRepository.UpdatePaymentID(id, session.PaymentIntentId);
                    unitOfWork.OrderHeaderRepository.UpdatePaymentStatus(id, SD.PAYMENT_STATUS_APPROVED);
                    unitOfWork.Save();
                }
            }

            return View(id);
        }
    }
}
