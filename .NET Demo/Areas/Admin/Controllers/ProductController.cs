using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    //TODO: Rework controllers in a base class


    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IWebHostEnvironment webHostEnvironment;
        private IProductRepository productRepo;
        
        private IEnumerable<SelectListItem> CategoryList
        {
            get => unitOfWork.CategoryRepository.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });
        }

        public ProductController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            productRepo = unitOfWork.ProductRepository;
        }

        private void AddOperationFeedback(string name, string objName = "Product")
        {
            TempData["success"] = $"{objName} {name} successfully";
        }

        private bool FindProduct(int? id, out Product product)
        {
            if (id == null || id == 0)
            {
                product = new Product();
                return false;
            }

            product = productRepo.GetFirstOrDefault(u => u.Id == id) ?? new Product();

            if (product == null)
            {
                return false;
            }

            return true;
        }

        public IActionResult Index()
        {
            var objList = productRepo.GetAll(includeProperties: "Category");
            return View(objList);
            //In order for @model to access the data on the cshtml,
            //the data sent as model must be inside the View method parameters
        }

        public IActionResult Upsert(int? id)
        {
            var vm = new ProductVM
            {
                Product = new Product(),
                CategoryList = CategoryList
            };

            if(id == null || id == 0)
            {
                return View(vm);
            }

            vm.Product = unitOfWork.ProductRepository.GetFirstOrDefault(p => p.Id == id);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM vm, IFormFile? file)
        {
            if (
                !string.IsNullOrEmpty(vm.Product.Title) &&
                productRepo.GetFirstOrDefault(c => c.Title.ToLower() == vm.Product.Title.ToLower()) != null)
            {
                ModelState.AddModelError("Name", $"A product with the name '{vm.Product.Title}' was already added.");
                ModelState.AddModelError("", $"A product with the name '{vm.Product.Title}' was already added");
            }

            if (ModelState.IsValid)
            {
                var wwwRootPath = webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(vm.Product.ImageUrl))
                    {
                        var oldPath = Path.Combine(wwwRootPath, vm.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    using (var stream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    vm.Product.ImageUrl = @"\images\product\" + fileName;  
                }

                if(vm.Product.Id == 0)
                {
                    productRepo.Add(vm.Product);
                }
                else
                {
                    productRepo.Update(vm.Product);
                }
                unitOfWork.Save();
                AddOperationFeedback("created");
                return RedirectToAction("Index");
            }

            vm.CategoryList = CategoryList;
            return View();
        }


        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = unitOfWork.ProductRepository.GetAll(includeProperties: "Category");
            return Json(new { data = list });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = unitOfWork.ProductRepository.GetFirstOrDefault(p => p.Id == id);
            if(obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var path = Path.Combine(webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(obj.ImageUrl))
            {
                System.IO.File.Delete(obj.ImageUrl);
            }

            unitOfWork.ProductRepository.Remove(obj);
            unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
