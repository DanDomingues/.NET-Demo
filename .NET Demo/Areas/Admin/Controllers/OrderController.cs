using Demo.DataAccess.Repository;
using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = SD.ROLE_USER_ADMIN)]
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
                
            UpdateRepo(
                orderHeader, 
                unitOfWork.OrderHeaderRepository.Update, 
                feedback: "Order Updated Sucessfully", 
                redirection: "Details", 
                redirectionArgs: new { orderId = orderHeader.Id });

            unitOfWork.Save();         
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            var all = Repo.GetAll(track: false, includeProperties: DefaultIncludeProperties);
            Func<OrderHeader, bool> filter;
            switch(status)
            {
                case "paymentpending":
                    filter = header => header.PaymentStatus == SD.PAYMENT_STATUS_PENDING;
                    break;
                case "inprocess":
                    filter = header => header.OrderStatus == SD.ORDER_STATUS_PROCESSING;
                    break;
                case "completed":
                    filter = header => header.OrderStatus == SD.PAYMENT_STATUS_APPROVED;
                    break;
                case "approved":
                    filter = header => header.OrderStatus == SD.ORDER_STATUS_APPROVED;
                    break;
                default:
                    filter = header => true;
                    break;

            }

            return Json(data: all.Where(filter));
        }
    }
}
