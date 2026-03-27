
using Demo.DataAccess.IRepository;
using Demo.Models;
using Microsoft.EntityFrameworkCore;
namespace Demo.DataAccess.Repository
{
    public class ShoppingCartItemRepository(DbSet<ShoppingCartItem> set) : Repository<ShoppingCartItem>(set), IShoppingCartItemRepository
    {
    }
}
