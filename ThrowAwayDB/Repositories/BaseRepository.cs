using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ThrowAwayDb;
namespace DataBase
{
     public class BaseRepository<T> : IRepository<T> 
     where T : BaseObject, new()
     {
        public virtual T GetById(int id)
        {
            var item  = new T();
            item.Id = id;
            //TODO: Sql SELECT item;
            return item;
        }
        public virtual IEnumerable<T> GetAll()
        {
            var list = new List<T>();
            //TODO: Sql SELECT record set.
            for(int i = 0; i < 5; i++)
            {
                var t = new T();
                t.Id = i;
                list.Add(t);
            }
            return list;
        }
        public virtual IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return new List<T>();
        }
        public virtual void Add(T entity)
        {   

        }
        public virtual void Delete(T entity)
        {

        }
        public virtual void Edit(T entity)
        {

        }
     }
}