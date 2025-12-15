using Demo.DataAccess.Data;
using Demo.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext db;
        private CategoryRepository category;
        private ProductRepository product;
        ICategoryRepository IUnitOfWork.CategoryRepository => category;
        IProductRepository IUnitOfWork.ProductRepository => product;

        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
            category = new CategoryRepository(db);
            product = new ProductRepository(db);
        }


        public void Save()
        {
            db.SaveChanges();
        }
    }
}
