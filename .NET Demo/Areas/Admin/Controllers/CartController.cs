using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    [Area("Customer")]
    public class CartController(IUnitOfWork unitOfWork) : RepositoryBoundController<ShoppingCart, IShoppingCartRepository>(unitOfWork)
    {
        protected override string DefaultFeedbackName => "Shopping Cart";
        protected override IShoppingCartRepository Repo => unitOfWork.ShoppingCarts;

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
    }
}
