using Northstar.DataAccess.Repository;
using Northstar.DataAccess.IRepository;
using Northstar.Models;
using Northstar.Models.ViewModels;
using Northstar.DataAccess;
using Northstar.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Northstar.Web.Controllers;

namespace Northstar.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController(IUnitOfWork unitOfWork) : RepositoryBoundController<OrderHeader, IOrderHeaderRepository>(unitOfWork)
    {
        protected override IOrderHeaderRepository Repo => unitOfWork.OrderHeaderRepository;
        protected override string DefaultFeedbackName => "Order";
        protected override string? DefaultIncludeProperties => "ApplicationUser";

        public IActionResult Index(string filter)
        {
            if((filter == "all" && User.HasAdminRights()) || filter == "user")
            {
                return View();
            }
            throw new ArgumentException("Invalid filter value. Expected 'all' or 'user'.");
        }

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
                Details = orderItems,
                Carriers = [.. SD.CARRIER_LIST]
            });
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


        [Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
        public IActionResult UpdateDetails(OrderVM vm)
        {
            var orderHeader = Repo.GetFirst(order => order.Id.Equals(vm.Header.Id), track: false);
               
            // Map fields from vm.Header to orderHeader
            var asContainer = orderHeader as IShippingContainer;
            asContainer.FetchDetails(vm.Header);
                            
            return UpdateRepo(
                orderHeader, 
                unitOfWork.OrderHeaderRepository.Update, 
                feedback: "Order Updated Sucessfully", 
                redirection: "Index");
        }

        [Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
        public IActionResult StartProcess(OrderVM vm)
        {
            var header = Repo.GetById(vm.Header.Id, track: true);
            
            header.Carrier = vm.Header.Carrier ?? header.Carrier;
            header.OrderStatus = SD.ORDER_STATUS_PROCESSING;
            header.TrackingNumber = Guid.NewGuid().ToString();
            this.AddSuccessFeedback("Order Details Changed Sucessfully");
            unitOfWork.Save();         

            return UpdateRepo(
                header,
                Repo.Update,
                feedback: "Order Shipped",
                redirection: "Details",
                redirectionArgs: new { id = header.Id });
        }

        [Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
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

        [Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
        public IActionResult CancelOrder(OrderVM vm)
        {
            var header = Repo.GetById(vm.Header.Id);

            if(header.PaymentStatus?.Equals(SD.PAYMENT_STATUS_APPROVED) == true)
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
        
        [Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
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
        public IActionResult GetAllBy(string status, string filter)
        {
            var all = GetAllByPermission(filter);

            if(!all.Any())
            {
                return Json(data: Array.Empty<OrderHeader>());
            }

            Func<OrderHeader, bool> statusFilter = status switch
            {
                "paymentpending" => header => header.PaymentStatus == SD.PAYMENT_STATUS_PENDING,
                "inprocess" => header => header.OrderStatus == SD.ORDER_STATUS_PROCESSING,
                "completed" => header => header.OrderStatus == SD.PAYMENT_STATUS_APPROVED,
                "approved" => header => header.OrderStatus == SD.ORDER_STATUS_APPROVED,
                _ => header => !string.IsNullOrEmpty(header.OrderStatus),
            };

            return Json(new { data = all.Where(statusFilter) });
        }

        private IEnumerable<OrderHeader> GetAllByPermission(string filter)
        {
            if(!User.TryGetId(out var userId))
            {
                return [];
            }

            if (filter == "all" && User.HasAdminRights())
            {
                return Repo.GetAll(
                    includeProperties: DefaultIncludeProperties,
                    track: false);
            }
            else if(filter == "user")
            {
                return Repo.GetAll(
                    header => header.ApplicationUserId == userId, 
                    includeProperties: DefaultIncludeProperties, 
                    track: false);
            }

            throw new ArgumentException("Invalid filter value or permissions. Expected 'all' or 'user' with appropriate permissions.");
        }
    }
}
