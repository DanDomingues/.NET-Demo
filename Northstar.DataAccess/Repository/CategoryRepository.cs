
using Northstar.DataAccess.IRepository;
using Northstar.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Northstar.DataAccess.Repository
{
    public class CategoryRepository(DbSet<Category> set) : Repository<Category>(set), ICategoryRepository
    {
    }
}
