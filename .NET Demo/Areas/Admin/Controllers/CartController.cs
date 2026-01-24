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

        public override IActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var vm = new ShoppingCartVM
            {
                ProductList = Repo.GetAll(e => e.ApplicationUser.Id == userId, includeProperties: "Product"),
            };
            return View(vm);
        }

        public IActionResult Summary()
        {
            return View();
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
