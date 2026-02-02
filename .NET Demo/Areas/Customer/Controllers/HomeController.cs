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
            var product = unitOfWork.ProductRepository.GetById(id, includeProperties: "Category", track: false);
            var cart = new ShoppingCartItem { Product = product, ProductId = product.Id, Count = 1 };
            return View(cart);
        }

        [HttpPost, Authorize]
        public IActionResult Details(ShoppingCartItem cart)
        {
            //TODO: Update to method used in cart controller
            var claims = User.Identity as ClaimsIdentity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            var existing = unitOfWork.ShoppingCarts.GetFirstOrDefault(c => c.ApplicationUserId == userId && c.Id == cart.Id, track: false);

            if (existing != null)
            {
                //Fair note here: By changing a property in an object extracted through framework core, we're changing the original object as well, not just a value copy
                //Therefore, there is no need to update (it would, if we had built a different object from the ground up)
                //To avoid this behavior, we can either do a copy of the existing cart that was retrieved or use .AsNoTracking in the Get method

                existing.Count += cart.Count;
                unitOfWork.ShoppingCarts.Update(existing);
            }
            else
            {
                //TODO: Review why this is happening
                //Somewhere, the Id for the cart is being bound to (apparently) be the same as the ProductId
                //As we're adding a new entry, the id needs to be 0 so the entity system can assign it
                cart.Id = 0;
                unitOfWork.ShoppingCarts.Add(cart);
            }

            //TODO: Add data feedback through TempData

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