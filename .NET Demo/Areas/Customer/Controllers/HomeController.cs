using Demo.DataAccess;
using Demo.Models;
using Demo.Utility;
using Demo.DataAccess.IRepository;
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
            var list = unitOfWork.ProductRepository.GetAll(includeProperties:"Category,Images");
            //Inits the session value to be used in the layout view
            //TODO: Test removing this and observe efffects
            this.RefreshCartItemsCount();

            return View(list);
        }

        public IActionResult Details(int id)
        {
            var product = unitOfWork.ProductRepository.GetById(
                id, 
                includeProperties: "Category,Images",
                track: false);
            var cart = new ShoppingCartItem { Product = product, ProductId = product.Id, Count = 1 };
            return View(cart);
        }

        [HttpPost, Authorize]
        public IActionResult Details(ShoppingCartItem cart)
        {
            if(!User.TryGetId(out var userId))
            {
                return View();
            }

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
                
                //Inits cart values
                cart.ApplicationUserId = userId;
                cart.Count = 1;

                //Adds to DB and saved
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