using DemoWithRazor.Data;
using DemoWithRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoWithRazor.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext db;
        [BindProperty]
        public Category Category { get; set; }

        public DeleteModel(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void OnGet(int? id)
        {
            if (id == null || id == 0)
            {
                return;
            }
            Category = db.Categories.Find(id);
        }

        public IActionResult OnPost(Category category)
        {
            db.Categories.Remove(Category);
            db.SaveChanges();
            TempData["success"] = $"Category deleted successfully";
            return RedirectToPage("Index");
        }
    }
}
