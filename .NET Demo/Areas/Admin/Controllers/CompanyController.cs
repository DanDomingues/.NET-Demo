using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_USER_ADMIN)]
    public class CompanyController(IUnitOfWork unitOfWork) : RepositoryBoundController<Company, ICompanyRepository>(unitOfWork)
    {
        protected override ICompanyRepository Repo => unitOfWork.CompanyRepository;
        protected override string DefaultFeedbackName => "Company";
    }
}
