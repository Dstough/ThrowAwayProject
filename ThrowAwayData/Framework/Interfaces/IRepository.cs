using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public interface IRepository<T> where T : BaseObject
    {
        IRepository<T> Where(object arg);
        T Get(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(int count = 0);
        void Add(T entity);
        void Delete(int id);
        void Edit(T entity);
    }
}