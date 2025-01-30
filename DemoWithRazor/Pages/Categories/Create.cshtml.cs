using DemoWithRazor.Data;
using DemoWithRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoWithRazor.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext db;
        [BindProperty]
        public Category category { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(Category category)
        {
            db.Categories.Add(category);
            db.SaveChanges();
            TempData["success"] = $"Category created successfully";
            return RedirectToPage("Index");
        }
    }
}
