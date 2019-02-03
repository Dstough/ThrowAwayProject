using System;
using System.Reflection;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public class BaseRepository<T> : IRepository<T>
    where T : BaseObject, new()
    {
        private string ConnString { get; set; }
        public BaseRepository() : this("")
        {
        }
        public BaseRepository(string _connectionString)
        {
            ConnString = _connectionString;
        }
        public virtual T GetById(int id)
        {
            var item = new T();
            var tableName = item.GetType().Name;
            var columnList = "";

            using (var conn = new SQLiteConnection(ConnString))
            using (var cmd = conn.CreateCommand())
            {
                foreach (var prop in item.GetType().GetProperties())
                    columnList += prop.Name + ",";

                cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + tableName + " WHERE Id = @Id;";
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    foreach (var prop in item.GetType().GetProperties().Where(e => e.CanWrite))
                    {
                        if (prop.Name == "Id")
                            prop.SetValue(item, (int)reader[prop.Name], null);
                        else
                            prop.SetValue(item, reader[prop.Name], null);
                    }
                }
            }

            return item;
        }
        public virtual IEnumerable<T> GetAll()
        {
            var list = new List<T>();
            var itemTemplate = new T();
            var tableName = itemTemplate.GetType().Name;
            var columnList = "";

            foreach (var prop in itemTemplate.GetType().GetProperties())
                columnList += prop.Name + ",";

            using (var conn = new SQLiteConnection(ConnString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + tableName + " WHERE Deleted = 0;";
                conn.Open();
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new T();
                            foreach (var prop in item.GetType().GetProperties().Where(e => e.CanWrite))
                            {
                                if (prop.Name == "Id")
                                    prop.SetValue(item, Convert.ToInt32(reader[prop.Name]), null);
                                else
                                    prop.SetValue(item, reader[prop.Name], null);
                            }
                            list.Add(item);
                        }
                    }
                }
                catch (SQLiteException)
                {
                    CreateTable(conn, tableName, itemTemplate.GetType().GetProperties());
                }
            }
            return list;
        }
        public virtual IEnumerable<T> Find(Func<T, bool> predicate)
        {
            //TODO: Fix this. It is lazy and incorrect in terms of how to handle a predicate function.
            return GetAll().Where(predicate);
        }
        public virtual void Add(T entity)
        {
            var tableName = entity.GetType().Name;
            var columns = "";
            var paramiters = "";

            using (var conn = new SQLiteConnection(ConnString))
            using (var cmd = conn.CreateCommand())
            {
                foreach (var prop in entity.GetType().GetProperties().Where(e => e.Name != "Id"))
                {
                    columns += prop.Name + ",";
                    paramiters += "@" + prop.Name + ",";
                    cmd.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(entity));
                }
                cmd.CommandText = "INSERT INTO " + tableName + " (" + columns.TrimEnd(',') + ")";
                cmd.CommandText += "VALUES (" + paramiters.TrimEnd(',') + ");";
                cmd.CommandText += "SELECT MAX (Id) FROM " + tableName + " AS Id;";
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return;
                    entity.Id = Convert.ToInt32(reader[0] ?? 0);
                }
            }
        }
        public virtual void Edit(T entity)
        {
            var tableName = entity.GetType().Name;

            using (var conn = new SQLiteConnection(ConnString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE " + tableName + " SET ";
                foreach (var prop in entity.GetType().GetProperties().Where(e => e.Name != "Id"))
                {
                    cmd.CommandText += prop.Name + "=@" + prop.Name + ",";
                    cmd.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(entity));
                }
                cmd.CommandText = cmd.CommandText.TrimEnd(',') + " WHERE Id = @Id;";
                cmd.Parameters.AddWithValue("@Id", entity.Id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public virtual void Delete(int id)
        {
            var itemTemplate = new T();
            var tableName = itemTemplate.GetType().Name;

            using (var conn = new SQLiteConnection(ConnString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE " + tableName + " SET Deleted = 1 WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        protected void CreateTable(SQLiteConnection conn, string tableName, PropertyInfo[] propertyList)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Create Table " + tableName + "(";
                foreach (var prop in propertyList)
                {
                    cmd.CommandText += prop.Name + " ";

                    if (prop.Name == "Id")
                        cmd.CommandText += "INTEGER PRIMARY KEY,";
                    else
                    {
                        switch (prop.PropertyType.Name.ToLower())
                        {
                            case "int": cmd.CommandText += "int,"; break;
                            case "string": cmd.CommandText += "varchar,"; break;
                            case "boolean": cmd.CommandText += "bit,"; break;
                            case "datetime": cmd.CommandText += "datetime,"; break;
                            case "double": cmd.CommandText += "double,"; break;
                            default: cmd.CommandText += "blob,"; break;
                        }
                    }
                }
                cmd.CommandText = cmd.CommandText.TrimEnd(',');
                cmd.CommandText += ");";
                cmd.ExecuteNonQuery();
            }
        }
    }
}