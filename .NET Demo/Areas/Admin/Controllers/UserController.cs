using ASP.NET_Debut.Areas.Admin.Controllers;
using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
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

        public override IActionResult Index()
        {
            return base.Index();
        }

        public override IActionResult Delete(int id)
        {
            return base.Delete(id);
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
    }
}
