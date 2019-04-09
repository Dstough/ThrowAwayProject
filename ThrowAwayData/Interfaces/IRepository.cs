using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public interface IRepository<T> where T : BaseObject
    {
        IRepository<T> Include(string include);
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetPage(int page, int size);
        IEnumerable<T> Find(IEnumerable<Filter> whereClause);
        void Add(T entity);
        void Delete(int id);
        void Edit(T entity);
        int Count(IEnumerable<Filter> filters);
    }
}