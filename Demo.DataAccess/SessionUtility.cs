using System.Security.Claims;
using Demo.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Demo.Utility;

namespace Demo.DataAccess
{
    public static class SessionUtility
    {
        public static int RefreshCartItemsCount(HttpContext context, IUnitOfWork unitOfWork, string userId)
        {
            var count = unitOfWork.ShoppingCarts.GetAll(item => item.ApplicationUserId.Equals(userId)).Count();
            context.Session.SetInt32(SD.CART_SESSION, count);
            return count;
        }

        public static int RefreshCartItemsCount(this Controller controller, IUnitOfWork unitOfWork)
        {
            return 
                controller.User.TryGetId(out var id) ?
                RefreshCartItemsCount(controller.HttpContext, unitOfWork, id) :
                -1;                 
        }

        public static int RefreshCartItemsCount<T>(this T controller) where T : Controller, IUnitOfWorkProvider
        {
            return controller.RefreshCartItemsCount(controller.UnitOfWork);
        }
    }

    public interface IUnitOfWorkProvider
    {
        public IUnitOfWork UnitOfWork { get; }
    }
}