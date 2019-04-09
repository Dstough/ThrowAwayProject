using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.SQLite;
using System.Collections.Generic;
using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public class BaseRepository<T> : IRepository<T>
    where T : BaseObject, new()
    {
        private string ConnString { get; set; }
        private List<string> IncludeFields { get; set; }

        public BaseRepository() : this("")
        {
        }

        public BaseRepository(string _connectionString)
        {
            ConnString = _connectionString;
            IncludeFields = new List<string>();
        }

        public virtual IRepository<T> Include(string name)
        {
            if (Array.IndexOf(typeof(T).GetProperties(), name) != -1)
                IncludeFields.Add(name);
            return this;
        }

        public virtual T GetById(int id)
        {
            var item = new T();
            var tableName = item.GetType().Name;
            var columnList = "";

            using (var conn = new SQLiteConnection(ConnString))
            using (var cmd = conn.CreateCommand())
            {
                foreach (var prop in item.GetType().GetProperties().Where(t => !t.GetType().IsSubclassOf(typeof(BaseObject))))
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
                        if (prop.PropertyType == typeof(int) || prop.Name == "Id")
                            prop.SetValue(item, Convert.ToInt32(reader[prop.Name]), null);
                        else
                            prop.SetValue(item, reader[prop.Name], null);
                    }
                }
                //TODO: factor in the IncludeFields into the property setting.
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

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new T();
                        foreach (var prop in item.GetType().GetProperties().Where(e => e.CanWrite))
                        {
                            if (prop.PropertyType == typeof(int) || prop.Name == "Id")
                                prop.SetValue(item, Convert.ToInt32(reader[prop.Name]), null);
                            else
                                prop.SetValue(item, reader[prop.Name], null);
                        }
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public virtual IEnumerable<T> GetPage(int page, int size)
        {
            var list = new List<T>();
            var itemTemplate = new T();
            var tableName = itemTemplate.GetType().Name;
            var columnList = "";
            var pageNumber = (page - 1) * size;

            foreach (var prop in itemTemplate.GetType().GetProperties())
                columnList += prop.Name + ",";

            using (var conn = new SQLiteConnection(ConnString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + tableName + " ORDER BY Id LIMIT " + size + " OFFSET " + pageNumber + ";";
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new T();
                        foreach (var prop in item.GetType().GetProperties().Where(e => e.CanWrite))
                        {
                            if (prop.PropertyType == typeof(int) || prop.Name == "Id")
                                prop.SetValue(item, Convert.ToInt32(reader[prop.Name]), null);
                            else
                                prop.SetValue(item, reader[prop.Name], null);
                        }
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        public virtual IEnumerable<T> Find(IEnumerable<Filter> filters)
        {
            var list = new List<T>();
            var itemTemplate = new T();
            var tableName = itemTemplate.GetType().Name;
            var columnList = "";
            var whereClause = "Deleted = 0";

            foreach (var prop in itemTemplate.GetType().GetProperties())
                columnList += prop.Name + ",";

            using (var conn = new SQLiteConnection(ConnString))
            using (var cmd = conn.CreateCommand())
            {
                //This might not be the best way to do the filter.
                //It shifts the responsiblily on the app to know when to use sql quotes for the filter values.
                //Maybe find another way in the futre.
                foreach (var filter in filters)
                {
                    if (columnList.Contains(filter.Column))
                    {
                        whereClause += " AND " + filter.Column + "=@" + filter.Column;
                        cmd.Parameters.AddWithValue("@" + filter.Column, filter.Value);
                    }
                }
                cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + tableName + " WHERE " + whereClause + ";";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new T();
                        foreach (var prop in item.GetType().GetProperties().Where(e => e.CanWrite))
                        {
                            if (prop.PropertyType == typeof(int) || prop.Name == "Id")
                                prop.SetValue(item, Convert.ToInt32(reader[prop.Name]), null);
                            else
                                prop.SetValue(item, reader[prop.Name], null);
                        }
                        list.Add(item);
                    }
                }
            }

            return list;
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

        public virtual int Count(IEnumerable<Filter> filters = null)
        {
            var count = 0;
            var itemTemplate = new T();
            var tableName = itemTemplate.GetType().Name;
            var whereClause = "Deleted = 0";
            var columnList = "";

            foreach (var prop in itemTemplate.GetType().GetProperties())
                columnList += prop.Name + ",";

            using (var conn = new SQLiteConnection(ConnString))
            using (var cmd = conn.CreateCommand())
            {
                //This might not be the best way to do the filter.
                //It shifts the responsiblily on the app to know when to use sql quotes for the filter values.
                //Maybe find another way in the futre.
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        if (columnList.Contains(filter.Column))
                        {
                            whereClause += " AND " + filter.Column + "=@" + filter.Column;
                            cmd.Parameters.AddWithValue("@" + filter.Column, filter.Value);
                        }
                    }
                }

                cmd.CommandText = "SELECT COUNT(*) as count FROM " + tableName + " WHERE " + whereClause + ";";

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        count = Convert.ToInt32(reader["count"]);
                }
            }
            return count;
        }
    }
}