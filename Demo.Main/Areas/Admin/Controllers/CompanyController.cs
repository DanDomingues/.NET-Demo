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

        public IActionResult Index() => Modules["Index"].Get();
        public IActionResult Upsert(int? id) => Modules["Upsert"].GetWithId(id);

        [HttpPost]
        public IActionResult Upsert(Company upsert)
        {
            return Modules["Upsert"].Post(upsert);
        }
    }
}
