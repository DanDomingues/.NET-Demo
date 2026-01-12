using Demo.DataAccess.Data;
using Demo.DataAccess.Repository.IRepository;
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
        private readonly CategoryRepository category = new(db.Categories);
        private readonly ProductRepository product = new(db.Products);
        private readonly CompanyRepository company = new(db.Companies);
        private readonly ShoppingCartRepository shoppingCart = new(db.ShoppingCarts);

        ICategoryRepository IUnitOfWork.CategoryRepository => category;
        IProductRepository IUnitOfWork.ProductRepository => product;
        ICompanyRepository IUnitOfWork.CompanyRepository => company;
        IShoppingCartRepository IUnitOfWork.ShoppingCartRepository => shoppingCart;

        public void Save()
        {
            db.SaveChanges();
        }
    }
}
