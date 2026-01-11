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

        public override IActionResult Create(Category model)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            if(CheckForDuplicates(model))
            {
                return View();
            }

            return base.Create(model);
        }
    }
}
