using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_USER_ADMIN)]
    public class ProductController(IUnitOfWork unitOfWork) : RepositoryBoundController<Product, IProductRepository>(unitOfWork)
    {
        private IWebHostEnvironment webHostEnvironment;
        
        private IEnumerable<SelectListItem> CategoryList
        {
            get => unitOfWork.CategoryRepository.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });
        }

        protected override IProductRepository Repo => unitOfWork.ProductRepository;

        protected override string DefaultFeedbackName => "Product";

        public override IActionResult Index()
        {
            var objList = Repo.GetAll(includeProperties: "Category");
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

        //TODO: Move upsert to the class above if needed
        [HttpPost]
        public IActionResult Upsert(ProductVM vm, IFormFile? file)
        {
            if (
                !string.IsNullOrEmpty(vm.Product.Title) &&
                Repo.GetFirstOrDefault(c => c.Title.ToLower() == vm.Product.Title.ToLower()) != null)
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
                    Repo.Add(vm.Product);
                }
                else
                {
                    Repo.Update(vm.Product);
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
