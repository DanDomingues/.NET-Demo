using Demo.DataAccess.Data;
using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DataAccess.Repository
{
    public class ShoppingCartItemRepository(DbSet<ShoppingCartItem> set) : Repository<ShoppingCartItem>(set), IShoppingCartItemRepository
    {
    }
}
