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
            //TODO: Select with reflection property generated SQL
            return item;
        }
        public virtual IEnumerable<T> GetAll()
        {
            var list = new List<T>();
            //TODO: Select with reflection property generated SQL
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
            //TODO: Select with reflection property generated SQL
            return new List<T>();
        }
        public virtual void Add(T entity)
        {   
            //TODO: Insert with reflection property generated SQL
        }
        public virtual void Delete(T entity)
        {
            //TODO: Delete with reflection property generated SQL
        }
        public virtual void Edit(T entity)
        {
            //TODO: Edit with reflection property generated SQL
        }

        public string TestSQL(T entity)
        {
            var sql = "INSERT INTO " + entity.GetType().Name + " ";
            var columnNames = "";
            var values = "";

            foreach(var prop in entity.GetType().GetProperties().Where(e=>e.Name != "Id"))
            {
                var parameterName = "@" + prop.Name;
                var parameterValue = prop.GetValue(entity,null);
                //TODO: cmd.paramiters.Add(parameterName, parameterValue);

                columnNames += prop.Name + ",";
                values += parameterName + ",";
            }
            sql += "(" + columnNames.TrimEnd(',') + ")\nVALUES (" + values.TrimEnd(',') + ");" ;
            return sql;
        }
     }
}