using Demo.DataAccess.IRepository;
using Demo.Main.Controllers;
using Demo.Main.Controllers.Modules;
using Demo.Models;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Main.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_USER_ADMIN)]
    public class CompanyController : RepositoryBoundController<Company, ICompanyRepository>
    {
        protected override ICompanyRepository Repo => unitOfWork.CompanyRepository;
        protected override string DefaultFeedbackName => "Company";

        public CompanyController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            Modules["Upsert"] = new RepoControllerUpsertModule<Company, ICompanyRepository>(this);
        }

        public IActionResult Index() => View();
        public IActionResult Upsert(int? id)
        {
            var company = Find(id, out var c) ? c : new Company();
            return PartialView("_UpsertModal", company);
        }

        [HttpPost]
        public IActionResult Upsert(Company model)
        {
            if (CheckForDuplicatesByName(model) || !ModelState.IsValid)
            {
                return Json(new { success = false });
            }

            UpdateRepo(model, Repo.AddOrUpdate);
            return Json(new { success = true });
        }

        public override IActionResult GetAll()
        {
            var all = Repo.GetAll();

            foreach (var company in all)
            {
                company.EmployeeCount = unitOfWork.ApplicationUserRepository
                    .GetAll(u => u.CompanyId == company.Id)
                    .Count();
            }

            return base.GetAll();
        }
    }
}
