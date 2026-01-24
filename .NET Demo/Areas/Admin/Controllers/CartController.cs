using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
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
                ApplicationUser = appUser,
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
