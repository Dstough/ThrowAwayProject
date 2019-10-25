using System;
using System.Linq;
using System.Data.SQLite;
using System.Collections;
using System.Collections.Generic;

namespace ThrowAwayDataBackground
{
    public abstract class BaseRepository<T> : IRepository<T>
    where T : BaseObject, new()
    {
        private string ConnString { get; set; }

        private Dictionary<string, Tuple<string, string>> WhereParameters { get; set; }

        private List<string> ApprovedOperators { get; set; }

        #region Constructors

        public BaseRepository() : this("")
        { }

        public BaseRepository(string _connectionString)
        {
            ConnString = _connectionString;
            IncludeFields = new List<string>();
            WhereParameters = new Dictionary<string, Tuple<string, string>>();
            ApprovedOperators = new List<string> { "||", "*", "/", "%", "+", "-", "<<", ">>", "&", "|", "<", "<=", ">", ">=", "=", "==", "!=", "<>", "IS", "IS NOT", "IN", "LIKE", "GLOB", "MATCH", "REGEXP", "AND", "OR" };
        }

        #endregion

        #region Core

        public virtual IRepository<T> Where(object obj)
        {
            var type = obj.GetType();
            var OperatorSymbol = "=";

            if (type.GetProperty("OperatorSymbol") != null && ApprovedOperators.Contains(obj.GetPropValue("OperatorSymbol")))
                OperatorSymbol = obj.GetPropValue("OperatorSymbol").ToString();

            foreach (var prop in obj.GetType().GetProperties())
            {
                if (typeof(T).GetProperty(prop.Name) == null)
                    continue;

                WhereParameters.Add(prop.Name, Tuple.Create(OperatorSymbol, obj.GetPropValue(prop.Name).ToString()));
            }

            return this;
        }

        public virtual T Get(int id)
        {
            var item = new T();
            var tableName = item.GetType().Name;
            var columnList = "";
            var propList = item.GetType()
                               .GetProperties()
                               .Where(t => !t.PropertyType.IsSubclassOf(typeof(BaseObject)))
                               .Where(t => !(t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                               .Where(e => e.CanWrite);

            using var conn = new SQLiteConnection(ConnString);
            using var cmd = conn.CreateCommand();

            foreach (var prop in propList)
                columnList += prop.Name + ",";

            cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + tableName + " WHERE Id = @Id" + " ORDER BY CreatedOn DESC;";
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            foreach (var prop in propList)
            {
                if (prop.PropertyType == typeof(int) || prop.Name == "Id")
                    prop.SetValue(item, Convert.ToInt32(reader[prop.Name]), null);
                else
                    prop.SetValue(item, reader[prop.Name], null);
            }

            item = IncludeProperties(item);
            WhereParameters.Clear();
            IncludeFields.Clear();

            return item;
        }

        public virtual T Get(string publicId)
        {
            var item = new T();
            var tableName = item.GetType().Name;
            var columnList = "";
            var propList = item.GetType()
                               .GetProperties()
                               .Where(t => !t.PropertyType.IsSubclassOf(typeof(BaseObject)))
                               .Where(t => !(t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                               .Where(e => e.CanWrite);

            using var conn = new SQLiteConnection(ConnString);
            using var cmd = conn.CreateCommand();

            foreach (var prop in propList)
                columnList += prop.Name + ",";

            cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + tableName + " WHERE PublicId = @PublicId" + " ORDER BY CreatedOn DESC;";
            cmd.Parameters.AddWithValue("@PublicId", publicId);
            conn.Open();

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            foreach (var prop in propList)
            {
                if (prop.PropertyType == typeof(int) || prop.Name == "Id")
                    prop.SetValue(item, Convert.ToInt32(reader[prop.Name]), null);
                else
                    prop.SetValue(item, reader[prop.Name], null);
            }

            item = IncludeProperties(item);
            WhereParameters.Clear();
            IncludeFields.Clear();

            return item;
        }

        public virtual IEnumerable<T> Find(int count = 0)
        {
            var list = new List<T>();
            var itemTemplate = new T();
            var tableName = itemTemplate.GetType().Name;
            var columnList = "";
            var whereClause = "Deleted = 0";
            var limitClause = count > 0 ? " LIMIT " + count : "";
            var propList = typeof(T).GetProperties()
                                    .Where(t => !t.PropertyType.IsSubclassOf(typeof(BaseObject)))
                                    .Where(t => !(t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                                    .Where(e => e.CanWrite);

            foreach (var prop in propList)
                columnList += prop.Name + ",";

            using var conn = new SQLiteConnection(ConnString);
            using var cmd = conn.CreateCommand();

            foreach (var paramiter in WhereParameters)
            {
                whereClause += " AND " + paramiter.Key + " " + paramiter.Value.Item1 + " @" + paramiter.Key;
                cmd.Parameters.AddWithValue("@" + paramiter.Key, paramiter.Value.Item2);
            }

            cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + tableName + " WHERE " + whereClause + " ORDER BY CreatedOn DESC " + limitClause + ";";

            conn.Open();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var item = new T();
                foreach (var prop in propList)
                {
                    if (prop.PropertyType == typeof(int) || prop.Name == "Id")
                        prop.SetValue(item, Convert.ToInt32(reader[prop.Name]), null);
                    else
                        prop.SetValue(item, reader[prop.Name], null);
                }
                list.Add(IncludeProperties(item));
            }

            IncludeFields.Clear();
            WhereParameters.Clear();

            return list;
        }

        public virtual void Add(T entity)
        {
            var tableName = entity.GetType().Name;
            var columns = "";
            var paramiters = "";
            var propList = typeof(T).GetProperties()
                                    .Where(t => !t.PropertyType.IsSubclassOf(typeof(BaseObject)))
                                    .Where(t => !(t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                                    .Where(e => e.Name != "Id");

            entity.PublicId = GetNewPublicId();

            using var conn = new SQLiteConnection(ConnString);
            using var cmd = conn.CreateCommand();

            foreach (var prop in propList)
            {
                columns += prop.Name + ",";
                paramiters += "@" + prop.Name + ",";
                cmd.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(entity));
            }
            cmd.CommandText = "INSERT INTO " + tableName + " (" + columns.TrimEnd(',') + ")";
            cmd.CommandText += "VALUES (" + paramiters.TrimEnd(',') + ");";
            cmd.CommandText += "SELECT MAX (Id) FROM " + tableName + " AS Id;";
            conn.Open();

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return;
            entity.Id = Convert.ToInt32(reader[0] ?? 0);

            IncludeFields.Clear();
            WhereParameters.Clear();
        }

        public virtual void Edit(T entity)
        {
            var tableName = entity.GetType().Name;
            var propList = typeof(T).GetProperties()
                                    .Where(t => !t.PropertyType.IsSubclassOf(typeof(BaseObject)))
                                    .Where(t => !(t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                                    .Where(e => e.Name != "Id");

            using var conn = new SQLiteConnection(ConnString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = "UPDATE " + tableName + " SET ";
            foreach (var prop in propList)
            {
                cmd.CommandText += prop.Name + "=@" + prop.Name + ",";
                cmd.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(entity));
            }
            cmd.CommandText = cmd.CommandText.TrimEnd(',') + " WHERE Id = @Id;";
            cmd.Parameters.AddWithValue("@Id", entity.Id);

            conn.Open();
            cmd.ExecuteNonQuery();

            IncludeFields.Clear();
            WhereParameters.Clear();
        }

        public virtual void Delete(int id)
        {
            var itemTemplate = new T();
            var tableName = itemTemplate.GetType().Name;

            using var conn = new SQLiteConnection(ConnString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = "UPDATE " + tableName + " SET Deleted = 1 WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();

            IncludeFields.Clear();
            WhereParameters.Clear();
        }

        #endregion

        #region Extra

        private List<string> IncludeFields { get; set; }

        public virtual IRepository<T> Include(string name)
        {
            if (typeof(T).GetProperty(name) != null)
                IncludeFields.Add(name);

            return this;
        }

        private T IncludeProperties(T item)
        {
            if (IncludeFields.Count() <= 0)
                return item;

            var columnList = "";
            using var conn = new SQLiteConnection(ConnString);
            using var cmd = conn.CreateCommand();

            conn.Open();
            foreach (var field in IncludeFields)
            {
                var prop = typeof(T).GetProperties().Where(p => p.Name == field).FirstOrDefault();

                if (prop == null)
                    throw new Exception("The property was not found in the data object.");

                if (!(prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    var subItem = Activator.CreateInstance(prop.PropertyType);
                    var subPropList = prop.PropertyType.GetProperties()
                                                       .Where(t => !t.PropertyType.IsSubclassOf(typeof(BaseObject)))
                                                       .Where(t => !(t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                                                       .Where(e => e.CanWrite);

                    foreach (var subProp in subPropList)
                        columnList += subProp.Name + ",";

                    cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + prop.Name + " WHERE Id = @Id;";
                    cmd.Parameters.AddWithValue("@Id", item.GetPropValue(prop.Name + "Id"));

                    using var reader = cmd.ExecuteReader();

                    if (!reader.Read())
                        return item;

                    foreach (var subProp in subPropList)
                    {
                        if (subProp.PropertyType == typeof(int) || subProp.Name == "Id")
                            subProp.SetValue(subItem, Convert.ToInt32(reader[subProp.Name]), null);
                        else
                            subProp.SetValue(subItem, reader[subProp.Name], null);
                    }

                    prop.SetValue(item, subItem);
                }
                else
                {
                    var listType = prop.PropertyType.GetAnyElementType();
                    var subItem = Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));
                    var listItem = Activator.CreateInstance(listType);
                    var subPropList = listType.GetProperties()
                                              .Where(t => !t.PropertyType.IsSubclassOf(typeof(BaseObject)))
                                              .Where(t => !(t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                                              .Where(e => e.CanWrite);

                    foreach (var subProp in subPropList)
                        columnList += subProp.Name + ",";

                    cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + listType.Name + " WHERE " + typeof(T).Name + "Id = @" + typeof(T).Name + "Id;";
                    cmd.Parameters.AddWithValue("@" + typeof(T).Name + "Id", item.GetPropValue("Id"));

                    using var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        foreach (var subProp in subPropList)
                        {
                            if (subProp.PropertyType == typeof(int) || subProp.Name == "Id")
                                subProp.SetValue(listItem, Convert.ToInt32(reader[subProp.Name]), null);
                            else
                                subProp.SetValue(listItem, reader[subProp.Name], null);
                        }
                        ((IList)subItem).Add(listItem);
                    }

                    prop.SetValue(item, subItem);
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
            var propList = typeof(T).GetProperties()
                                    .Where(t => !t.PropertyType.IsSubclassOf(typeof(BaseObject)))
                                    .Where(t => !(t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                                    .Where(e => e.CanWrite);

            foreach (var prop in propList)
                columnList += prop.Name + ",";

            using var conn = new SQLiteConnection(ConnString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + tableName + " WHERE Deleted = 0;";
            conn.Open();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var item = new T();
                foreach (var prop in propList)
                {
                    if (prop.PropertyType == typeof(int) || prop.Name == "Id")
                        prop.SetValue(item, Convert.ToInt32(reader[prop.Name]), null);
                    else
                        prop.SetValue(item, reader[prop.Name], null);
                }
                list.Add(IncludeProperties(item));
            }


            IncludeFields.Clear();
            WhereParameters.Clear();

            return list;
        }

        public virtual int Count()
        {
            var count = 0;
            var itemTemplate = new T();
            var tableName = itemTemplate.GetType().Name;
            var whereClause = "Deleted = 0";
            var columnList = "";
            var propList = typeof(T).GetProperties()
                                    .Where(t => !t.PropertyType.IsSubclassOf(typeof(BaseObject)))
                                    .Where(t => !(t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)));

            foreach (var prop in propList)
                columnList += prop.Name + ",";

            using var conn = new SQLiteConnection(ConnString);
            using var cmd = conn.CreateCommand();

            foreach (var paramiter in WhereParameters)
            {
                whereClause += " AND " + paramiter.Key + " " + paramiter.Value.Item1 + " @" + paramiter.Key;
                cmd.Parameters.AddWithValue("@" + paramiter.Key, paramiter.Value.Item2);
            }

            cmd.CommandText = "SELECT COUNT(" + columnList.TrimEnd(',') + ") as count FROM " + tableName + " WHERE " + whereClause + ";";

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
                count = Convert.ToInt32(reader["count"]);

            IncludeFields.Clear();
            WhereParameters.Clear();

            return count;
        }

        public virtual IEnumerable<T> GetPage(int page, int size)
        {
            var list = new List<T>();
            var itemTemplate = new T();
            var tableName = itemTemplate.GetType().Name;
            var columnList = "";
            var pageNumber = (page - 1) * size;
            var propList = typeof(T).GetProperties()
                                    .Where(t => !t.PropertyType.IsSubclassOf(typeof(BaseObject)))
                                    .Where(t => !(t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                                    .Where(e => e.CanWrite);

            foreach (var prop in propList)
                columnList += prop.Name + ",";

            using var conn = new SQLiteConnection(ConnString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + tableName + " WHERE Deleted = 0 ORDER BY Id LIMIT " + size + " OFFSET " + pageNumber + ";";
            conn.Open();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var item = new T();
                foreach (var prop in propList)
                {
                    if (prop.PropertyType == typeof(int) || prop.Name == "Id")
                        prop.SetValue(item, Convert.ToInt32(reader[prop.Name]), null);
                    else
                        prop.SetValue(item, reader[prop.Name], null);
                }
                list.Add(IncludeProperties(item));
            }

            IncludeFields.Clear();
            WhereParameters.Clear();

            return list;
        }

        public virtual T GetRandom()
        {
            var item = new T();
            var tableName = item.GetType().Name;
            var whereClause = "Deleted = 0";
            var columnList = "";
            var propList = typeof(T).GetProperties()
                                    .Where(t => !t.PropertyType.IsSubclassOf(typeof(BaseObject)))
                                    .Where(t => !(t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)));

            foreach (var prop in propList)
                columnList += prop.Name + ",";

            using var conn = new SQLiteConnection(ConnString);
            using var cmd = conn.CreateCommand();

            foreach (var paramiter in WhereParameters)
            {
                whereClause += " AND " + paramiter.Key + " " + paramiter.Value.Item1 + " @" + paramiter.Key;
                cmd.Parameters.AddWithValue("@" + paramiter.Key, paramiter.Value.Item2);
            }

            cmd.CommandText = "SELECT " + columnList.TrimEnd(',') + " FROM " + tableName + " WHERE " + whereClause + " ORDER BY RANDOM() LIMIT 1";
            conn.Open();

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            foreach (var prop in propList)
            {
                if (prop.PropertyType == typeof(int) || prop.Name == "Id")
                    prop.SetValue(item, Convert.ToInt32(reader[prop.Name]), null);
                else
                    prop.SetValue(item, reader[prop.Name], null);
            }

            return item;
        }

        protected virtual string GetNewPublicId()
        {
            var tableName = typeof(T).Name;

            while (true)
            {
                var value = "".RandomString(11);
                using var conn = new SQLiteConnection(ConnString);
                using var cmd = conn.CreateCommand();
                
                cmd.CommandText = "SELECT PublicId FROM " + tableName + " WHERE PublicId = @publicId";
                cmd.Parameters.AddWithValue("@publicId", value);

                conn.Open();

                using var reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return value;
            }
        }

        #endregion
    }
}