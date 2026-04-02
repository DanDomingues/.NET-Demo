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

        public IActionResult MoveUp(int? id) => Move(id, i => i - 1);
        
        public IActionResult MoveDown(int? id) => Move(id, i => i + 1);

        public IActionResult Move(int? id, Func<int, int> getNewIndex)
        {
            void OnNewList(Category[] categories)
            {
                for (int i = 0; i < categories.Length; i++)
                {
                    if(categories[i].DisplayOrder != i)
                    {
                        categories[i].DisplayOrder = i;
                        Repo.Update(categories[i]);
                    }
                }

                unitOfWork.Save();
            }

            return GenericUtility.MoveInList<Category, int, IActionResult>(
                element: Find(id, out var c, track: false) ? c : null,
                list: [.. Repo.GetAll(track: false).OrderBy(c => c.DisplayOrder)],
                compare: (c1, c2) => c1.Id.Equals(c2.Id),
                getKey: c => c.Id,
                getNewIndex: getNewIndex,
                onSuccess: () => RedirectToAction(nameof(Index)),
                onFail: message => Json(new { success = false, message }),
                onNewList: OnNewList);
        }
    }
}
