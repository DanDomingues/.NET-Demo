
using Northstar.DataAccess.IRepository;
using Northstar.Models;
using Microsoft.EntityFrameworkCore;
namespace Northstar.DataAccess.Repository
{
    public class ShoppingCartItemRepository(DbSet<ShoppingCartItem> set) : Repository<ShoppingCartItem>(set), IShoppingCartItemRepository
    {
    }
}
