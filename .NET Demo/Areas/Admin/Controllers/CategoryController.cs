using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_USER_ADMIN)]
    public class CategoryController : Controller
    {
        IUnitOfWork unitOfWork;
        ICategoryRepository CategoryRepo => unitOfWork.CategoryRepository;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private void AddOperationFeedback(string name, string objName = "Category")
        {
            TempData["success"] = $"{objName} {name} successfully";
        }

        private bool FindCategory(int? id, out Category category)
        {
            if (id == null || id == 0)
            {
                category = new Category();
                return false;
            }

            category = CategoryRepo.GetFirstOrDefault(u => u.Id == id) ?? new Category();

            if (category == null)
            {
                return false;
            }

            return true;
        }

        public IActionResult Index()
        {
            var objList = CategoryRepo.GetAll();
            return View(objList);
            //In order for @model to access the data on the cshtml,
            //the data sent as model must be inside the View method parameters
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!string.IsNullOrEmpty(category.Name) &&
                CategoryRepo.GetFirstOrDefault(c => c.Name.ToLower() == category.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Name", $"A category with the name '{category.Name}' was already added.");
                ModelState.AddModelError("", $"A category with the name '{category.Name}' was already added");
            }

            if (ModelState.IsValid)
            {
                CategoryRepo.Add(category);
                unitOfWork.Save();
                AddOperationFeedback("created");
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (!FindCategory(id, out var category))
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                CategoryRepo.Update(category);
                unitOfWork.Save();
                AddOperationFeedback("edited");
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (!FindCategory(id, out var category))
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if (!FindCategory(id, out var category))
            {
                return NotFound();
            }

            CategoryRepo.Remove(category);
            unitOfWork.Save();
            AddOperationFeedback("deleted");
            return RedirectToAction("Index");
        }
    }
}
