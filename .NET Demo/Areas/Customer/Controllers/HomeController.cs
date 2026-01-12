using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ASP.NET_Debut.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var list = unitOfWork.ProductRepository.GetAll(includeProperties:"Category");
            return View(list);
        }

        public IActionResult Details(int id)
        {
            var product = unitOfWork.ProductRepository.GetById(id, includeProperties: "Category");
            var cart = new ShoppingCart { Product = product, ProductId = product.Id, Count = 1 };
            return View(cart);
        }

        [HttpPost, Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claims = User.Identity as ClaimsIdentity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            var existing = unitOfWork.ShoppingCarts.GetFirstOrDefault(c => c.ApplicationUserId == userId && c.Id == cart.Id);
            if (existing != null)
            {
                existing.Count += cart.Count;
                unitOfWork.ShoppingCarts.Update(existing);
            }
            else
            {
                unitOfWork.ShoppingCarts.Add(cart);
            }

            unitOfWork.Save();
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