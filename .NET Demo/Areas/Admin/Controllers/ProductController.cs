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
    public class ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) : RepositoryBoundController<Product, IProductRepository>(unitOfWork)
    {
        private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;
        
        protected override IProductRepository Repo => unitOfWork.ProductRepository;
        protected override string DefaultFeedbackName => "Product";
        protected override string? DefaultIncludeProperties => "Category";

        private IEnumerable<SelectListItem> CategoryList
        {
            get => unitOfWork.CategoryRepository.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });
        }

        public override IActionResult Upsert(int? id)
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

            vm.Product = Repo.GetById(id);
            return View(vm);
        }

        public IActionResult UpsertVM(int? id)
        {
            return Upsert(id);
        }

        [HttpPost]
        public IActionResult UpsertVM(ProductVM vm, IFormFile? file)
        {
            vm.Product.Name ??= vm.Product.Title;

            //To be removed once ImageUrl works well enough
            vm.Product.ImageUrl ??= string.Empty;

            if (!ModelState.IsValid && ValidateForUpsert(vm.Product))
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }
        
            if (file != null)
            {
                UpsertProductImage(vm.Product, file);
            }

            return Upsert(vm.Product);
        }

        private void UpsertProductImage(Product product, IFormFile file)
        {
            var wwwRootPath = webHostEnvironment.WebRootPath;
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var productPath = Path.Combine(wwwRootPath, @"images\product");

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var oldPath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            using (var stream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }
            
            product.ImageUrl = @"\images\product\" + fileName;
        }


        #region API Calls

        [HttpDelete]
        public override IActionResult Delete(int id)
        {
            var obj = Repo.GetById(id);
            if (obj == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Error while deleting"
                });
            }

            var path = Path.Combine(
                webHostEnvironment.WebRootPath, 
                obj.ImageUrl.TrimStart('\\'));    
            
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            return base.Delete(id);
        }
        #endregion
    }
}
