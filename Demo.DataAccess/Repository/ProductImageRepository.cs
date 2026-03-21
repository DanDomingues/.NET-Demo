using Demo.Models;
using Microsoft.EntityFrameworkCore;
using Demo.DataAccess.IRepository;

namespace Demo.DataAccess.Repository
{
    public class ProductImageRepository(DbSet<ProductImage> dbSet) : Repository<ProductImage>(dbSet), IProductImageRepository
    {
        
    }
}