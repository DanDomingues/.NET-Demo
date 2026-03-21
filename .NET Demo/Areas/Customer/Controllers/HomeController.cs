using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP.NET_Debut.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) : Controller, IUnitOfWorkProvider
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        public IUnitOfWork UnitOfWork => unitOfWork;

        public IActionResult Index()
        {
            var list = unitOfWork.ProductRepository.GetAll(includeProperties:"Category");
            //Inits the session value to be used in the layout view
            //TODO: Test removing this and observe efffects
            this.RefreshCartItemsCount();

            return View(list);
        }

        public IActionResult Details(int id)
        {
            var product = unitOfWork.ProductRepository.GetById(id, includeProperties: "Category", track: false);
            var cart = new ShoppingCartItem { Product = product, ProductId = product.Id, Count = 1 };
            return View(cart);
        }

        [HttpPost, Authorize]
        public IActionResult Details(ShoppingCartItem cart)
        {
            var userId = User.GetUserId();

            var existing = unitOfWork.ShoppingCarts.GetFirstOrDefault(
                c => c.ApplicationUserId == userId && c.ProductId == cart.ProductId, 
                track: false);

            if (existing != null)
            {
                existing.Count += cart.Count;
                unitOfWork.ShoppingCarts.Update(existing);
                unitOfWork.Save();
            }
            else
            {
                //TODO: Review why this is happening
                //Somewhere, the Id for the cart is being bound to (apparently) be the same as the ProductId
                //As we're adding a new entry, the id needs to be 0 so the entity system can assign it
                cart.Id = 0;
                unitOfWork.ShoppingCarts.Add(cart);
                unitOfWork.Save();

                //Updates the session value, to be used in the view
                this.RefreshCartItemsCount();
            }
         
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}