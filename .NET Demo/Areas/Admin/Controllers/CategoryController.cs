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

        [HttpPost]
        public override IActionResult Create(Category model)
        {
            if(CheckForDuplicates(model, Repo))
            {
                return View();
            }

            return base.Create(model);
        }
    }
}
