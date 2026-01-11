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
    public class CategoryController(IUnitOfWork unitOfWork) : RepositoryBoundController<Category, ICategoryRepository>(unitOfWork)
    {
        protected override ICategoryRepository Repo => unitOfWork.CategoryRepository;

        protected override string DefaultFeedbackName => "Category";

        public override IActionResult Create(Category obj)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            if (!string.IsNullOrEmpty(obj?.Name))
            {
                var duplicate = Repo.GetFirstOrDefault(
                    c => string.Equals(c.Name.ToLower(), obj.Name.ToLower()));

                if (duplicate != null)
                {
                    ModelState.AddModelError(
                        "Name", 
                        $"A category with the name '{obj.Name}' was already added.");
                    ModelState.AddModelError(
                        "", 
                        $"A category with the name '{obj.Name}' was already added");
                }
            }

            return base.Create(obj);
        }
    }
}
