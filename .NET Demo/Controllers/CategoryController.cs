using ASP.NET_Debut.Controllers.Data;
using ASP.NET_Debut.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Debut.Controllers
{
    public class CategoryController : Controller
    {
        ApplicationDbContext db;

        public CategoryController(ApplicationDbContext db)
        {
            this.db = db;
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

            category = db.Categories.Find(id) ?? new Category();

            if (category == null)
            {
                return false;
            }

            return true;
        }

        public IActionResult Index()
        {
            var objList = db.Categories.ToList();
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
            if (!string.IsNullOrEmpty(category.Name) && db.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", $"A category with the name '{category.Name}' was already added.");
                ModelState.AddModelError("", $"A category with the name '{category.Name}' was already added");
            }

            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
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
                db.Categories.Update(category);
                db.SaveChanges();
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
            if(!FindCategory(id, out var category))
            {
                return NotFound();
            }

            db.Categories.Remove(category);
            db.SaveChanges();
            AddOperationFeedback("deleted");
            return RedirectToAction("Index");
        }
    }
}
