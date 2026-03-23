using Demo.DataAccess.IRepository;
using ASP.NET_Debut.Controllers;
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

        protected override bool ValidateForUpsert(Product model)
        {
            return base.ValidateForUpsert(model) && !CheckForDuplicatesByName(model);
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

            vm.Product = Repo.GetById(id, includeProperties: "Category,Images");
            return View(vm);
        }

        public IActionResult UpsertVM(int? id)
        {
            return Upsert(id);
        }

        [HttpPost]
        public IActionResult UpsertVM(ProductVM vm, List<IFormFile>? files)
        {
            vm.Product.Name ??= vm.Product.Title;

            if (!ModelState.IsValid || !ValidateForUpsert(vm.Product))
            {
                return View(vm);
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            //For our product to have an assigned ID, upset to DB must come first
            //TODO: Validate if vm.Product gets an assigned ID after being used for DB updating
            //var output = Upsert(vm.Product);
            Repo.AddOrUpdate(vm.Product);
            unitOfWork.Save();
            var prodFromDb = Repo.GetFirstOrDefault(p => p.Name.Equals(vm.Product.Name));

            if (files != null)
            {
                UpsertProductImage(prodFromDb, files);
                unitOfWork.Save();
            }
        
            return Upsert(prodFromDb);
        }

        private void UpsertProductImage(Product product, List<IFormFile> files)
        {
            if(!ModelState.IsValid)
            {
                return;
            }
            
            var wwwRootPath = webHostEnvironment.WebRootPath;
            var productPath = Path.Combine(wwwRootPath, @$"images\products\product-{product.Name}");
            
            if(!Directory.Exists(productPath))
            {
                Directory.CreateDirectory(productPath);
            }
            
            foreach (var file in files)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";       

                using (var stream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var image = new ProductImage
                {
                    Url = @"\" + productPath + @"\" + file,
                    ProductId = product.Id,
                };
                
                if (!string.IsNullOrEmpty(image.Url))
                {
                    var oldPath = Path.Combine(wwwRootPath, image.Url.TrimStart('\\'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }          

                product.Images ??= new();
                product.Images.Add(image);
                unitOfWork.ProductImagesRepository.Add(image);
            }
        }

        public IActionResult DeleteImage(int? id)
        {
            var image = unitOfWork.ProductImagesRepository.GetById(id);
            if(!string.IsNullOrEmpty(image?.Url))
            {
                var fullPath = Path.Combine(webHostEnvironment.WebRootPath, image.Url.TrimStart('\\'));
                if(System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                unitOfWork.ProductImagesRepository.Remove(image);
                unitOfWork.Save();
            }

            this.AddOperationFeedback("Image Deleted Successfully");
            return RedirectToAction(nameof(UpsertVM), image?.ProductId);
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

            var wwwRootPath = webHostEnvironment.WebRootPath;
            var productPath = Path.Combine(wwwRootPath, @$"images\products\product-{obj.Name}");

            if(Directory.Exists(productPath))
            {
                Directory.Delete(productPath, true);
            }

            foreach (var image in obj.Images)
            {
                unitOfWork.ProductImagesRepository.Remove(image);
            }

            return base.Delete(id);
        }
        #endregion
    }
}
