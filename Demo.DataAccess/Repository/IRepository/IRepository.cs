using Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, bool track = true, string? includeProperties = null);
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, bool track = true, string? includeProperties = null);
        T GetById(int? id, bool track = true, string? includeProperties = null);
        void Add(T entity);
        void Update(T entity);
        void AddOrUpdate(T entity);
        void Remove(T entity);
        void RemoveRange(T[] entities);
    }
}
