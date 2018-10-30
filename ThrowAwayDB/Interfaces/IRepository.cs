using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace DataBase
{
    public interface IRepository<T> where T : BaseObject
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Edit(T entity);
    }
}