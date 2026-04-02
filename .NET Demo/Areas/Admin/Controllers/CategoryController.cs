using Demo.DataAccess.IRepository;
using Demo.Main.Controllers;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Main.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_USER_ADMIN)]
    public class CategoryController(IUnitOfWork unitOfWork) : RepositoryBoundController<Category, ICategoryRepository>(unitOfWork)
    {
        protected override ICategoryRepository Repo => unitOfWork.CategoryRepository;

        protected override string DefaultFeedbackName => "Category";

        public IActionResult Index()
        {
            var categories = Repo.GetAll(includeProperties: DefaultIncludeProperties);
            var VMs = categories
                .Select(c => new CategoryViewModel
                {
                    Category = c,
                    ProductCount = unitOfWork.ProductRepository.GetAll(p => p.CategoryId == c.Id).Count()
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
            if (CheckForDuplicatesByName(model))
            {
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UpdateRepo(model, Repo.AddOrUpdate);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            return Find(id, out var category, track: true)
                ? Modules["Delete"].PostWithId(id)
                : RedirectToAction(nameof(Index));
        }
    }
}
