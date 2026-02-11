using Demo.DataAccess.Repository;
using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Demo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Debut.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = SD.ROLE_USER_ADMIN)]
    public class OrderController(IUnitOfWork unitOfWork) : RepositoryBoundController<OrderHeader, IOrderHeaderRepository>(unitOfWork)
    {
        protected override IOrderHeaderRepository Repo => unitOfWork.OrderHeaderRepository;

        protected override string DefaultFeedbackName => "Order";

        protected override string? DefaultIncludeProperties => "ApplicationUser";

        public override IActionResult Index()
        {
            return base.Index();
        }
    }
}
