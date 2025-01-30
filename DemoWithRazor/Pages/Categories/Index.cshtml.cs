using DemoWithRazor.Data;
using DemoWithRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoWithRazor.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext db;
        public List<Category> categories;


        public IndexModel(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void OnGet()
        {
            categories = db.Categories.ToList();
        }
    }
}
