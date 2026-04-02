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
            var category = Find(id, out var c, track: true) ? c : throw new ArgumentException("Invalid id");
            var categories = Repo.GetAll(track: false).OrderBy(c => c.DisplayOrder).ToList();
            var inList = categories.FirstOrDefault(c => c.Id == category.Id);

            if(inList == null)
            {
                return Json(new { success = false, message = "Not found in list" });
            }

            var index = categories.IndexOf(inList);
            var newIndex = getNewIndex(index);

            if(index < 0 || index >= categories.Count || newIndex < 0 || newIndex >= categories.Count)
            {
                return Json(new { success = false, message = "Invalid move operation" });
            }

            var categoriesDict = categories.ToDictionary(c => c.Id);
            categories.RemoveAt(index);
            categories.Insert(newIndex, category);

            foreach (var local in categories)
            {
                var newDisplayOrder = categories.IndexOf(local);
                if(newDisplayOrder != local.DisplayOrder)
                {
                    local.DisplayOrder = newDisplayOrder;
                    Repo.Update(local);
                }
            }

            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public TOut MoveInList<TModel, TKey, TOut>(
            TModel element,
            List<TModel> list,
            Func<int, int> getNewIndex,
            Func<TModel, TModel, bool> compare,
            Func<TModel, TKey> getKey,
            Action<TModel[]> onNewList,
            Func<TOut> onSuccess,
            Func<string, TOut> onFail
            ) where TKey : IComparable
        {
            var inList = list.FirstOrDefault(e => compare(e, element));

            if(inList == null)
            {
                return onFail("Element not present in list");
            }

            var index = list.IndexOf(inList);
            var newIndex = getNewIndex(index);

            if(index < 0 || index >= list.Count || newIndex < 0 || newIndex >= list.Count)
            {
                return onFail("Invalid move operation");
            }

            var categoriesDict = list.ToDictionary(c => getKey(c));
            list.RemoveAt(index);
            list.Insert(newIndex, element);
            onNewList([.. list]);
            return onSuccess();
        }
    }
}
