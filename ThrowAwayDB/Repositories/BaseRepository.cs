using System;
using System.Reflection;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ThrowAwayDb;
namespace DataBase
{
    public class BaseRepository<T> : IRepository<T>
    where T : BaseObject, new()
    {
        private const string ConnString = "Server=localhost;Database=ThrowAwayDB;Trusted_Connection=True;";

        public virtual T GetById(int id)
        {
            var item = new T();
            var tableName = item.GetType().Name;
            var columnList = "";

            using (var conn = new SqlConnection(ConnString))
            using (var cmd = conn.CreateCommand())
            {
                foreach (var prop in item.GetType().GetProperties())
                {
                    columnList += prop.Name + ",";
                }
                cmd.CommandText = "SELECT TOP 1 " + columnList.TrimEnd(',') + " FROM " + tableName + " WHERE Id = @Id;";
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    foreach (var prop in item.GetType().GetProperties().Where(e => e.CanWrite))
                    {
                        prop.SetValue(item, reader[prop.Name], null);
                    }
                }
            }

            return item;
        }
        public virtual IEnumerable<T> GetAll()
        {
            //TODO: Select with reflection property generated SQL
            var list = new List<T>();
            var itemTemplate = new T();
            var tableName = itemTemplate.GetType().Name;
            var columnList = "";

            foreach (var prop in itemTemplate.GetType().GetProperties())
            {
                columnList += prop.Name + ",";
            }
            
            using (var conn = new SqlConnection(ConnString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + tableName + ";";
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new T();
                        foreach (var prop in item.GetType().GetProperties().Where(e => e.CanWrite))
                        {
                            prop.SetValue(item, reader[prop.Name], null);
                        }
                        list.Add(item);
                    }
                }
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

            foreach (var prop in entity.GetType().GetProperties().Where(e => e.Name != "Id"))
            {
                var parameterName = "@" + prop.Name;
                var parameterValue = prop.GetValue(entity, null);
                //TODO: cmd.paramiters.Add(parameterName, parameterValue);

                columnNames += prop.Name + ",";
                values += parameterName + ",";
            }
            sql += "(" + columnNames.TrimEnd(',') + ")\nVALUES (" + values.TrimEnd(',') + ");";
            return sql;
        }
    }
}