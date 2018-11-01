using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace ThrowAwayDbBackground
{
    public interface IRepository<T> where T : BaseObject
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(int id);
        void Edit(T entity);
    }
}