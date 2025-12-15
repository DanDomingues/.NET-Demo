using Demo.DataAccess.Data;
using Demo.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext db;
        protected DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            this.db = db;
            dbSet = db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(T entity)
        {
            dbSet.RemoveRange(entity);
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            return GetAllWithProperties(dbSet, includeProperties).Where(filter).FirstOrDefault();
        }

        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            return GetAllWithProperties(dbSet, includeProperties).ToList();
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        private IQueryable<T> GetAllWithProperties(IQueryable<T> query, string? includeProperties = null)
        {
            if(!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query;
        }
    }
}
