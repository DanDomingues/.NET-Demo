using Demo.DataAccess.Data;
using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DataAccess.Repository
{
    public class Repository<T>(DbSet<T> dbSet) : IRepository<T> where T : ModelBase
    {
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public void AddOrUpdate(T entity)
        {
            if(entity.Id == 0)
            {
                Add(entity);
            }
            else
            {
                Update(entity);
            }
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
            return GetAllWithProperties(dbSet, includeProperties)
                .Where(filter)
                .FirstOrDefault();
        }

        public T GetById(int? id, string? includeProperties = null)
        {
            return GetFirstOrDefault(e => e.Id == id, includeProperties);
        }

        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            return GetAllWithProperties(dbSet, includeProperties).ToList();
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
