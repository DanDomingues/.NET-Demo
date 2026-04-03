using Demo.DataAccess;
using Demo.DataAccess.IRepository;
using Demo.Main.Controllers;
using Demo.Main.Controllers.Modules;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace Demo.Main.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_USER_ADMIN)]
    public class ProductController : RepositoryBoundController<Product, IProductRepository>
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) : base(unitOfWork)
        {
            this.webHostEnvironment = webHostEnvironment;
            Modules["Upsert"] = new RepoControllerUpsertModule<Product, IProductRepository>(this);
        }

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

        public IActionResult Index() => View();

        public IActionResult Upsert(int? id)
        {
            if(id == null || id == 0)
            {
                throw new InvalidOperationException("Invalid ID");
            }
            
            var product = Repo.GetById(id, includeProperties: "Category,Images");
            product.Images = [.. product.Images.OrderBy(i => i.DisplayOrder)];

            return View(new ProductVM
            {
                Product = product,
                CategoryList = CategoryList
            });
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM vm, List<IFormFile>? files)
        {
            vm.Product.Name ??= vm.Product.Title;

            if (!ModelState.IsValid || !ValidateForUpsert(vm.Product))
            {
                return View(vm);
            }

            //For our product to have an assigned ID, upset to DB must come first
            Repo.AddOrUpdate(vm.Product);
            unitOfWork.Save();

            if (files != null && files.Count > 0)
            {
                AddProductImages(vm.Product, files);
                unitOfWork.Save();
            }
        
            return Modules["Upsert"].Post(vm.Product);
        }

        private void AddProductImages(Product product, List<IFormFile> files)
        {
            if(!ModelState.IsValid)
            {
                return;
            }
            
            var productPath = @$"images\products\product-{product.Name}";
            var wwwRootPath = webHostEnvironment.WebRootPath;
            var localDirectoryPath = Path.Combine(wwwRootPath, productPath);
            
            if(!Directory.Exists(localDirectoryPath))
            {
                Directory.CreateDirectory(localDirectoryPath);
            }
            
            foreach (var file in files)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";       
                var filePath = Path.Combine(localDirectoryPath, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }          

                var image = new ProductImage
                {
                    Url = $@"\{productPath}\{fileName}",
                    ProductId = product.Id,
                    DisplayOrder = product.Images.Count,
                };

                product.Images ??= [];
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
            return RedirectToAction(nameof(Upsert), new { id = image?.ProductId });
        }

        public IActionResult MoveImageUp(int? prodId, int? imageId) => MoveImage(prodId, imageId, i => i - 1);
        
        public IActionResult MoveImageDown(int? prodId, int? imageId) => MoveImage(prodId, imageId, i => i + 1);

        public IActionResult MoveImageToTop(int? prodId, int? imageId) => MoveImage(prodId, imageId, i => 0);

        public IActionResult MoveImage(int? prodId, int? imageId, Func<int, int> getNewIndex)
        {
            var productImages = unitOfWork.ProductImagesRepository
                .GetAll(i => i.ProductId.Equals(prodId), track: false)
                .OrderBy(i => i.DisplayOrder);

            var image = productImages.FirstOrDefault(i => i.Id.Equals(imageId));

            return DataUtility.MoveInList<ProductImage, int, IActionResult>(
                unitOfWork: unitOfWork,
                repo: unitOfWork.ProductImagesRepository,
                element: image,
                list: [.. productImages],
                getNewIndex: getNewIndex,
                onSuccess: () => RedirectToAction(nameof(Upsert), new { id = prodId.ToString() }),
                onFail: message => Json(new { success = false, message }));
        }

        #region API Calls
        [HttpDelete]
        public override IActionResult DeleteAt(int id)
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

            if(obj.Images != null && obj.Images.Count > 0)
            {
                foreach (var image in obj.Images)
                {
                    unitOfWork.ProductImagesRepository.Remove(image);
                }                
            }

            return base.DeleteAt(id);
        }
        #endregion
    }
}
