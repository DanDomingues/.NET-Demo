using Demo.DataAccess.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DataAccess.Repository
{
    public class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
    {
        private readonly ApplicationDbContext db = db;
        private readonly ApplicationUserRepository applicationUser = new(db.ApplicationUsers);
        private readonly CategoryRepository categories = new(db.Categories);
        private readonly ProductRepository products = new(db.Products);
        private readonly ProductImageRepository productImages = new(db.ProductImages);
        private readonly CompanyRepository companies = new(db.Companies);
        private readonly ShoppingCartItemRepository cartItems = new(db.CartItems);
        private readonly OrderItemDetailsRepository orderItems = new(db.OrderItems);
        private readonly OrderHeaderRepository orderHeaders = new(db.OrderHeaders);

        ICategoryRepository IUnitOfWork.CategoryRepository => categories;
        IProductRepository IUnitOfWork.ProductRepository => products;
        IProductImageRepository IUnitOfWork.ProductImagesRepository => productImages;
        ICompanyRepository IUnitOfWork.CompanyRepository => companies;
        IShoppingCartItemRepository IUnitOfWork.ShoppingCarts => cartItems;
        IApplicationUserRepository IUnitOfWork.ApplicationUserRepository => applicationUser;
        IOrderItemDetailsRepository IUnitOfWork.OrderItemDetailsRepository => orderItems;
        IOrderHeaderRepository IUnitOfWork.OrderHeaderRepository => orderHeaders;

        public void Save()
        {
            db.SaveChanges();
        }
    }
}
