using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public interface IRepository<T> where T : BaseObject
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(IEnumerable<Filter> whereClause);
        void Add(T entity);
        void Delete(int id);
        void Edit(T entity);
    }
}