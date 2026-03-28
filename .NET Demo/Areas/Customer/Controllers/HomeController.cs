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
    public class HomeController(IUnitOfWork unitOfWork) : Controller, IUnitOfWorkProvider
    {
        IUnitOfWork IUnitOfWorkProvider.UnitOfWork => unitOfWork;
        private const string ProductIncludeProperties = "Category,Images";

        public IActionResult Index()
        {
            var list = unitOfWork.ProductRepository.GetAll(includeProperties: ProductIncludeProperties);
            //Inits the session value to be used in the layout view
            //TODO-2: Test removing this and observe efffects
            this.RefreshCartItemsCount();

            return View(list);
        }

        public IActionResult Details(int id)
        {
            User.TryGetId(out var userId);

            var product = unitOfWork.ProductRepository.GetById(
                id, 
                includeProperties: ProductIncludeProperties,
                track: false);
            
            var cart = new ShoppingCartItem 
            { 
                Product = product, 
                ProductId = product.Id,
                ApplicationUserId = userId,
            };

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
                //As binding may assign the directly available id through routing, we reset it to 0
                //Entity id needs to be at 0 prior to adding to a DB through EF
                
                //TODO-1: Add a VM instead of editing the cart item directly to avoid having the cart id pre-set
                cart.Id = 0;
                
                //Adds to DB and save
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