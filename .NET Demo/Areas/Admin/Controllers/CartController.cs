using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Mvc;
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
                    Count = item.Count,
                    Price = item.TotalCost,
                    OrderHeaderId = vm.OrderHeader.Id,
                });   
            }

            if(!isCompanyUser)
            {
                throw new NotImplementedException();
            }

            unitOfWork.Save();

            return RedirectToAction(nameof(OrderConfirmation), vm.OrderHeader.Id);
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
