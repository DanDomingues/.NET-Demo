using Demo.DataAccess;
using Demo.DataAccess.IRepository;
using ASP.NET_Debut.Controllers;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> um,
        RoleManager<IdentityRole> rm) : RepositoryBoundController<ApplicationUser, IApplicationUserRepository>(unitOfWork), IUnitOfWorkProvider
    {
        public IUnitOfWork UnitOfWork => unitOfWork;

        protected override IApplicationUserRepository Repo => unitOfWork.ApplicationUserRepository;
        protected override string DefaultFeedbackName => "User";
        protected override string? DefaultIncludeProperties => "Company";

        public IActionResult RoleManagement(string id)
        {
            var user = Repo
                .GetFirstOrDefault(u => u.Id == id);
            var companies = unitOfWork.CompanyRepository
                .GetAll(track: false)
                .Select(v => new SelectListItem(v.Name, v.Id.ToString()));
            var roles = rm.Roles
                .Select(r => new SelectListItem(r.Name, r.Name));
                
            return View(new RoleManagementVM
            {
                User = user,
                Companies = companies,
                Roles = roles
            });
        }

        [HttpPost] public IActionResult RoleManagement(RoleManagementVM vm)
        {   
            //First we retrieve the user from DB to compare and update   
            var userFromDb = Repo.GetFirstOrDefault(u => u.Id.Equals(vm.User.Id));

            //Then fetch role data and IDs from associated DBs
            var prevRoleName = userFromDb.Role;
            //var newRoleId = unitOfWork.DB.Roles.FirstOrDefault(r => r.Name.Equals(vm.User.Role)).Id;
            //var userRole = unitOfWork.DB.UserRoles.FirstOrDefault(u => u.UserId.Equals(vm.User.Id));

            //Finally the role is updated
            //userRole.RoleId = newRoleId;
            if(!string.IsNullOrEmpty(prevRoleName))
            {
                um.RemoveFromRoleAsync(userFromDb, prevRoleName).GetAwaiter().GetResult();
            }
            um.AddToRoleAsync(userFromDb, vm.User.Role).GetAwaiter().GetResult();

            //And if needed, a company is assigned/unassigned
            if(vm.User.Role == SD.ROLE_USER_COMPANY)
            {
                userFromDb.CompanyId = vm.User.CompanyId;                
            }
            else if(vm.User.Role != prevRoleName && prevRoleName == SD.ROLE_USER_COMPANY)
            {
                userFromDb.CompanyId = null;
            }

            //Remove when role stops being tracked
            userFromDb.Role = vm.User.Role;

            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public override IActionResult GetAll()
        {
            //TODO-1: When role stops being tracked, it will have to be inserted here

            /*
            var userRoles = unitOfWork.DB.Roles.ToList();
            var userRoleIds = unitOfWork.DB.UserRoles.ToList();
            */

            var users = Repo.GetAll(track: false, includeProperties: DefaultIncludeProperties).Select(u =>
            {
                u.Company ??= new() { Name = "" };
                u.Locked = u.LockoutEnd != null && u.LockoutEnd.Value > DateTime.Now;
                return u;
            });

            return Json(new
            {
                data = users
            });
        }

        [HttpPost]
        public IActionResult ToggleLock([FromBody] string id)
        {
            var fromDb = Repo.GetFirstOrDefault(u => u.Id == id, track: true);

            if(fromDb == null)
            {
                return Json(new { success = false, message = "Error while setting account Lock" });
            }

            if(fromDb.LockoutEnd != null && fromDb.LockoutEnd.Value > DateTime.Now)
            {
                fromDb.LockoutEnd = null;
            }
            else
            {
                fromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            unitOfWork.Save();

            return Json(new { success = true, message = "Lock updated successfuly" });
        }
    }

}
