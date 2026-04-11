using Northstar.DataAccess;
using Northstar.DataAccess.IRepository;
using Northstar.Web.Controllers;
using Northstar.Models;
using Northstar.Models.ViewModels;
using Northstar.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Northstar.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController(
        IUnitOfWork unitOfWork,
        UserManager<IdentityUser> um,
        RoleManager<IdentityRole> rm) : RepositoryBoundController<ApplicationUser, IApplicationUserRepository>(unitOfWork), IUnitOfWorkProvider
    {
        public IUnitOfWork UnitOfWork => unitOfWork;

        protected override IApplicationUserRepository Repo => unitOfWork.ApplicationUserRepository;
        protected override string DefaultFeedbackName => "User";
        protected override string? DefaultIncludeProperties => "Company";

        public IActionResult Index() => Modules["Index"].Get();

        [HttpGet]
        public IActionResult ManageRole(string id)
        {
            var user = Repo
                .GetFirstOrDefault(u => u.Id == id);

            if(user == null)
            {
                throw new Exception("User not found");
            }

            user.Role = um
                .GetRolesAsync(user)
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault() ?? string.Empty;

            var companies = unitOfWork.CompanyRepository
                .GetAll(track: false)
                .Select(v => new SelectListItem(v.Name, v.Id.ToString()));
            
            var roles = rm.Roles
                .Select(r => new SelectListItem(r.Name, r.Name));
                
            return PartialView("_ManageRoleModal", new ManageRoleVM
            {
                User = user,
                Companies = companies,
                Roles = roles
            });
        }

        [HttpPost] 
        public IActionResult ManageRole(ManageRoleVM vm)
        {   
            // Load the persisted user so role and company changes are applied to the tracked entity.
            var userFromDb = Repo.GetFirst(u => u.Id.Equals(vm.User.Id));

            // Resolve the current role before replacing it.
            var prevRoleName = um.GetRolesAsync(userFromDb).GetAwaiter().GetResult().FirstOrDefault() ?? string.Empty;

            // Replace the existing role assignment with the selected one.
            if(!string.IsNullOrEmpty(prevRoleName))
            {
                um.RemoveFromRoleAsync(userFromDb, prevRoleName).GetAwaiter().GetResult();
            }
            um.AddToRoleAsync(userFromDb, vm.User.Role).GetAwaiter().GetResult();

            // Company-backed roles require a company reference; other roles should not keep one.
            if(vm.User.Role.EqualsAny(SD.ROLE_USER_COMPANY, SD.ROLE_USER_EMPLOYEE))
            {
                userFromDb.CompanyId = vm.User.CompanyId;                
            }
            else if(vm.User.Role != prevRoleName && prevRoleName.EqualsAny(SD.ROLE_USER_COMPANY, SD.ROLE_USER_EMPLOYEE))
            {
                userFromDb.CompanyId = null;
            }

            unitOfWork.Save();
            return Json(new { success = true });
        }

        #region API CALLS
        public override IActionResult GetAll()
        {
            var users = Repo.GetAll(track: false, includeProperties: DefaultIncludeProperties).Select(u =>
            {
                u.Company ??= new() { Name = "" };
                u.Locked = u.LockoutEnd != null && u.LockoutEnd.Value > DateTime.Now;
                u.Role = um.GetRolesAsync(u).GetAwaiter().GetResult().FirstOrDefault() ?? string.Empty;
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
    #endregion
}
