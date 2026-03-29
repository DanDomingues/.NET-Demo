using Demo.DataAccess.IRepository;
using ASP.NET_Debut.Controllers;
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

        public IActionResult Index() => IndexInternal();
        public IActionResult Edit(int? id) => EditInternal(id);
        public IActionResult Delete(int? id) => DeleteInternal(id);

        [HttpPost]
        public IActionResult Create(Category model)
        {
            if(CheckForDuplicatesByName(model))
            {
                return View();
            }

            return CreateInternalOnPost(model);
        }
    }
}
