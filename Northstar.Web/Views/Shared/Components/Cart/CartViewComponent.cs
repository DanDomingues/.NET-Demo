using Northstar.DataAccess.IRepository;
using Microsoft.AspNetCore.Mvc;
using Northstar.Utility;
using Northstar.DataAccess;

namespace Demo
{
    public class CartViewComponent(IUnitOfWork unitOfWork) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            if(User.TryGetId(out var id))
            {
                var count = SessionUtility.RefreshCartItemsCount(HttpContext, unitOfWork, id);
                return View(count);
            }
            else
            {
                HttpContext.Session.SetInt32(SD.CART_SESSION, 0);
                return View(0);
            }
        }
    }
}