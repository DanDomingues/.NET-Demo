using Demo.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Demo.Utility;

namespace Demo.ViewComponents
{
    public class CartViewComponent(IUnitOfWork unitOfWork) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var id = User?.GetUserId();
            if(id != null)
            {
                var count = SessionUtility.RefreshCartItemsCount(HttpContext, unitOfWork, id);
                return View(count);
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}