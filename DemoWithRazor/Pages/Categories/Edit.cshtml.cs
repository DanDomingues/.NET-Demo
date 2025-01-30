using DemoWithRazor.Data;
using DemoWithRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoWithRazor.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext db;
        [BindProperty]
        public Category Category { get; set; }

        public EditModel(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void OnGet(int? id)
        {
            if(id == null || id == 0)
            {
                return;
            }
            Category = db.Categories.Find(id);
        }

        public IActionResult OnPost()
        {
            db.Categories.Update(Category);
            db.SaveChanges();
            TempData["success"] = $"Category edited successfully";
            return RedirectToPage("Index");
        }
    }
}
