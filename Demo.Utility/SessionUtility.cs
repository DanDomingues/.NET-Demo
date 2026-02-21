

using System.Security.Claims;
using Demo.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Utility
{
    public static class SessionUtility
    {
        public static void RefreshCartItemsCount(HttpContext context, IUnitOfWork unitOfWork, string userId)
        {
            context.Session.SetInt32(
                SD.CART_SESSION, 
                unitOfWork.ShoppingCarts.GetAll(item => item.ApplicationUserId.Equals(userId)).Count());
        }

        public static void RefreshCartItemsCount(this Controller controller, IUnitOfWork unitOfWork)
        {
            RefreshCartItemsCount(controller.HttpContext, unitOfWork, controller.User.GetUserId());
        }

        public static void RefreshCartItemsCount<T>(this T controller) where T : Controller, IUnitOfWorkProvider
        {
            controller.RefreshCartItemsCount(controller.UnitOfWork);
        }
    }

    public interface IUnitOfWorkProvider
    {
        public IUnitOfWork UnitOfWork { get; }
    }
}