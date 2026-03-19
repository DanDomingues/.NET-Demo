using ASP.NET_Debut.Areas.Admin.Controllers;
using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Debut.Areas.Customer.Controllers
{
    [Area("Admin")]
    public class UserController(IUnitOfWork unitOfWork) : RepositoryBoundController<ApplicationUser, IApplicationUserRepository>(unitOfWork), IUnitOfWorkProvider
    {
        public IUnitOfWork UnitOfWork => unitOfWork;

        protected override IApplicationUserRepository Repo => unitOfWork.ApplicationUserRepository;
        protected override string DefaultFeedbackName => "User";
        protected override string? DefaultIncludeProperties => "Company";

        public IActionResult RoleManagement(int userId)
        {
            var user = Repo.GetById(userId);
            var companies = unitOfWork.CompanyRepository.GetAll(track: false);
            var roles = unitOfWork.DB.Roles.Select(v => v.Name);
            return View(new RoleManagementVM
            {
                User = user,
                Companies = companies,
                Roles = roles
            });
        }

        public override IActionResult GetAll()
        {
            //TODO: When role stops being tracked, it will have to be inserted here

            /*
            var userRoles = unitOfWork.DB.Roles.ToList();
            var userRoleIds = unitOfWork.DB.UserRoles.ToList();
            */

            var users = Repo.GetAll(track: false, includeProperties: DefaultIncludeProperties).Select(u =>
            {
                u.Company ??= new() { Name = "Unassigned" };
                return u;
            });

            return Json(new
            {
                data = users
            });
        }

        [HttpPost]
        public IActionResult ToggleLock([FromBody] int id)
        {
            var fromDb = Repo.GetById(id, track: true);

            if(fromDb == null)
            {
                return Json(new { success = false, message = "Error while setting account Lock" });
            }

            fromDb.LockoutEnabled = !fromDb.LockoutEnabled;
            unitOfWork.Save();

            return Json(new { success = true, message = "Lock updated successfuly" });
        }
    }

}
