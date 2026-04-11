using Northstar.Models;
using Microsoft.EntityFrameworkCore;
using Northstar.DataAccess.IRepository;

namespace Northstar.DataAccess.Repository
{
    public class ProductImageRepository(DbSet<ProductImage> dbSet) : Repository<ProductImage>(dbSet), IProductImageRepository
    {
        
    }
}