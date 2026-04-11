using Demo.DataAccess;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Demo.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Demo.Main.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController(IUnitOfWork unitOfWork) : Controller, IUnitOfWorkProvider
    {
        IUnitOfWork IUnitOfWorkProvider.UnitOfWork => unitOfWork;
        private const string ProductIncludeProperties = "Category,Images";

        public IActionResult Index()
        {
            var categories = unitOfWork.CategoryRepository.GetAll().OrderBy(c => c.DisplayOrder);
            var products = unitOfWork.ProductRepository.GetAll(includeProperties: ProductIncludeProperties);
            var vm = new ProductsHomeViewModel
            {
                Products = products,
                Categories = categories.ToDictionary(c => c.Id),
                CategoryProducts = categories.ToDictionary(
                    c => c.Id, 
                    c => products.Where(p => p.CategoryId == c.Id)),
            };

            return View(vm);
        }

        public IActionResult Details(int id)
        {
            User.TryGetId(out var userId);

            var product = unitOfWork.ProductRepository.GetById(
                id, 
                includeProperties: ProductIncludeProperties,
                track: false);

            return View(new ShoppingCartItemVM 
            { 
                Product = product,
                ProductId = product.Id,
                ApplicationUserId = userId,
                Quantity = 1
            });
        }

        [HttpPost, Authorize]
        public IActionResult Details(ShoppingCartItemVM vm)
        {
            if(!User.TryGetId(out var userId))
            {
                return this.RedirectToLogin();
            }

            var existing = unitOfWork.ShoppingCarts.GetFirstOrDefault(
                c => c.ApplicationUserId == userId && c.ProductId == vm.ProductId, 
                track: false);

            if (existing != null)
            {
                existing.Count += vm.Quantity;
                unitOfWork.ShoppingCarts.Update(existing);
                unitOfWork.Save();
            }
            else
            {
                //Adds to DB and save
                unitOfWork.ShoppingCarts.Add(new()
                {
                    ProductId = vm.ProductId,
                    ApplicationUserId = userId,
                    Count = vm.Quantity
                });
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