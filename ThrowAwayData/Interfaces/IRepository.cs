using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace ThrowAwayDataBackground
{
    public interface IRepository<T> where T : BaseObject
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Dictionary<string,string> whereClause);
        void Add(T entity);
        void Delete(int id);
        void Edit(T entity);
    }
}