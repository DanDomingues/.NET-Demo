using Demo.DataAccess.IRepository;
using Demo.Main.Controllers;
using Demo.Models;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Main.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_USER_ADMIN)]
    public class CompanyController(IUnitOfWork unitOfWork) : RepositoryBoundController<Company, ICompanyRepository>(unitOfWork)
    {
        protected override ICompanyRepository Repo => unitOfWork.CompanyRepository;
        protected override string DefaultFeedbackName => "Company";

        public IActionResult Index() => Modules["Index"].Get();
        public IActionResult Upsert(int? id) => Modules["Upsert"].GetWithId(id);

        [HttpPost]
        public IActionResult Upsert(Company upsert)
        {
            return Modules["Upsert"].Post(upsert);
        }
    }
}
