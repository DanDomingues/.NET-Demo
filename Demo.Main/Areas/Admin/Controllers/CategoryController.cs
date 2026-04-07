using Demo.DataAccess;
using Demo.DataAccess.IRepository;
using Demo.Main.Controllers;
using Demo.Main.Controllers.Modules;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Main.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_USER_ADMIN)]
    public class CategoryController : RepositoryBoundController<Category, ICategoryRepository>
    {
        public CategoryController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            Modules["Delete"] = new RepoControllerDeleteModule<Category, ICategoryRepository>(this);
        }

        protected override ICategoryRepository Repo => unitOfWork.CategoryRepository;

        protected override string DefaultFeedbackName => "Category";

        private int GetProductCount(Category category)
        {
            return unitOfWork.ProductRepository.GetAll(p => p.CategoryId == category.Id).Count();
        }

        public IActionResult Index()
        {
            var categories = Repo.GetAll(includeProperties: DefaultIncludeProperties);
            var VMs = categories
                .Select(c => new CategoryViewModel
                {
                    Category = c,
                    ProductCount = GetProductCount(c)
                })
                .OrderBy(c => c.Category.DisplayOrder)
                .ThenBy(c => c.ProductCount);

            return View(VMs);
        }

        public IActionResult Upsert(int? id)
        {
            var category = Find(id, out var c) ? c : new Category();
            return PartialView("_UpsertModal", category);
        }

        public IActionResult Delete(int? id) => Modules["Delete"].GetWithId(id);

        [HttpPost]
        public IActionResult Upsert(Category model)
        {
            if (CheckForDuplicatesByName(model) || !ModelState.IsValid)
            {
                return Json(new { success = false });
            }

            if(model.Id == 0)
            {
                model.DisplayOrder = Repo.GetAll(track: false).Count();
            }

            UpdateRepo(model, Repo.AddOrUpdate);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var products = unitOfWork.ProductRepository.GetAll(p => p.CategoryId == id, track: false);
            
            if(products.Any())
            {
                this.AddErrorFeedback("cannot be removed while there are associated products", "Category");
                return RedirectToAction(nameof(Index));
            }

            var deleted = Repo.GetById(id, track: false);
            var others = Repo.GetAll(e => e.DisplayOrder > deleted.DisplayOrder, track: false);

            foreach (var item in others)
            {
                item.DisplayOrder--;
                Repo.Update(item);
            }

            //Base delete functionality will call unitOfWork.Save
            return Modules["Delete"].PostWithId(id);
        }

        public IActionResult MoveUp(int? id) => Move(id, i => i - 1);
        
        public IActionResult MoveDown(int? id) => Move(id, i => i + 1);

        public IActionResult Move(int? id, Func<int, int> getNewIndex)
        {
            return DataUtility.MoveInList<Category, int, IActionResult>(
                unitOfWork: unitOfWork,
                repo: Repo,
                element: Find(id, out var c, track: false) ? c : null,
                list: [.. Repo.GetAll(track: false).OrderBy(c => c.DisplayOrder)],
                getNewIndex: getNewIndex,
                onSuccess: () => RedirectToAction(nameof(Index)),
                onFail: message => Json(new { success = false, message }));
        }
    }
}
