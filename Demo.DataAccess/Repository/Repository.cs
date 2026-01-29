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
    public class Repository<T>(DbSet<T> dbSet) : IRepository<T> where T : class, IModelBase
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

        public void RemoveRange(T[] entities)
        {
            dbSet.RemoveRange(entities);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, bool track = false, string? includeProperties = null)
        {
            var query = GetAllWithProperties(dbSet, includeProperties);
            if(!track)
            {
                query = query.AsNoTracking();
            }

            filter ??= _ => true;
            return [.. query.Where(filter)];
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, bool track = false, string? includeProperties = null)
        {
            return GetAll(filter, track, includeProperties).FirstOrDefault();
        }

        public T GetById(int? id, bool tracked = false, string? includeProperties = null)
        {
            return GetFirstOrDefault(e => e.Id == id, tracked, includeProperties);
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
