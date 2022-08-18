using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Grpc.Domain.Model;
using Grpc.Infrastructure.SqlServer;

namespace Grpc.Infrastructure.Repository
{
    public abstract class RepositoryDaoBase<T>
    : IRepository<T> where T : EntityBase, new()
    {
        protected SqlDatabase SqlDatabase { get; }

        public RepositoryDaoBase(string connectionString)
        {
            SqlDatabase = new SqlDatabase(connectionString);
        }

        public virtual async Task<bool> Delete(T entity)
        {
            return await Delete(entity.Id).ConfigureAwait(false);
        }

        public virtual async Task<bool> Delete(object id)
        {
            var defaultInstance = new T();
            var query = $"DELETE FROM {GetTableName(defaultInstance)} WHERE {GetPropertyKey(defaultInstance)} = @Id";

            int result;
            try
            {
                SqlDatabase.Connect();
                var sqlCommand = new SqlCommand(query, SqlDatabase.SqlConnection);
                sqlCommand.Parameters.Add(id);
                result = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

            }
            catch
            {
                throw;
            }
            finally
            {
                SqlDatabase.Disconnect();
            }

            return result > 0;
        }

        public virtual async Task<object> ExecuteRawSql(string query, params object[] parameters)
        {
            var result = default(object);

            try
            {
                SqlDatabase.Connect();
                var sqlCommand = new SqlCommand(query, SqlDatabase.SqlConnection);
                AddCommandParameters(sqlCommand, parameters);

                if (query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    using var dataReader = await sqlCommand.ExecuteReaderAsync().ConfigureAwait(false);
                    var records = new List<object>();

                    while (await dataReader.ReadAsync())
                    {
                        records.Add(GetRecordWithoutType(dataReader));
                    }

                    result = records;
                }
                else
                {
                    result = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                SqlDatabase.Disconnect();
            }

            return result;
        }

        public virtual async Task<List<T>> Get(int page = 1,
            int pageSize = 25,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null)
        {
            var defaultInstance = new T();
            var fields = string.Join(", ", GetProperties(defaultInstance));

            var query = $"SELECT TOP {pageSize} {fields} FROM {GetTableName(defaultInstance)}";

            var result = new List<T>();

            try
            {
                SqlDatabase.Connect();
                var sqlCommand = new SqlCommand(query, SqlDatabase.SqlConnection);
                using var dataReader = await sqlCommand.ExecuteReaderAsync().ConfigureAwait(false);

                while (await dataReader.ReadAsync())
                {
                    result.Add(GetRecord(dataReader));
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                SqlDatabase.Disconnect();
            }

            return result;
        }

        public virtual async Task<T> GetById(object id)
        {
            var result = await Get(pageSize: 1, filter: f => f.Id == id);
            return result.First();
        }

        public virtual async Task<bool> Insert(T entity)
        {
            var properties = GetProperties(entity);
            var fields = string.Join(", ", properties);
            var parameters = string.Join(", @", properties);

            var query = $"INSERT INTO {GetTableName(entity)} ({fields}) VALUES ({parameters})";

            int result = 0;

            try
            {
                SqlDatabase.Connect();
                var sqlCommand = new SqlCommand(query, SqlDatabase.SqlConnection);
                AddCommandParameters(sqlCommand, entity);

                result = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
            finally
            {
                SqlDatabase.Disconnect();
            }

            return result > 0;
        }

        public virtual async Task<bool> Update(T entity)
        {
            var properties = GetProperties(entity);
            var fields = string.Join(", ", properties, " = @", properties);

            var query = $"UPDATE {GetTableName(entity)} SET {fields}";

            int result = 0;
            try
            {
                SqlDatabase.Connect();
                var sqlCommand = new SqlCommand(query, SqlDatabase.SqlConnection);
                AddCommandParameters(sqlCommand, entity);

                result = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
            finally
            {
                SqlDatabase.Disconnect();
            }

            return result > 0;
        }

        protected List<string> GetProperties(T entity)
        {
            var properties = new List<string>();

            foreach (var item in entity.GetType().GetProperties())
            {
                if (item.CustomAttributes.All(f => f.AttributeType != typeof(NotMappedAttribute)))
                    properties.Add(item.Name);
            }

            return properties;
        }

        protected string GetPropertyKey(T entity)
        {
            string property = "";

            foreach (var item in entity.GetType().GetProperties())
            {
                if (item.CustomAttributes.Any(f => f.AttributeType == typeof(KeyAttribute)))
                    property = item.Name;
            }

            return property;
        }

        private T GetRecord(IDataReader dataReader)
        {
            var record = new T();

            foreach (var item in record.GetType().GetProperties())
            {
                if (item.CustomAttributes.All(f => f.AttributeType != typeof(NotMappedAttribute)) && dataReader[item.Name] != DBNull.Value)
                    item.SetValue(record, dataReader[item.Name]);
            }

            return record;
        }

        private object GetRecordWithoutType(IDataReader dataReader)
        {
            var record = new List<object>();
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                record.Add(dataReader[i]);
            }

            return record;
        }

        private void AddCommandParameters(SqlCommand sqlCommand, T entity)
        {
            foreach (var item in entity.GetType().GetProperties())
            {
                sqlCommand.Parameters.AddWithValue(item.Name, item.GetValue(entity));
            }
        }

        private void AddCommandParameters(SqlCommand sqlCommand, params object[] parameters)
        {
            foreach (var item in parameters)
            {
                sqlCommand.Parameters.Add(item);
            }
        }

        protected string GetTableName(T entity)
        {
            return $"{entity.Namespace}{(entity.Namespace == default ? "" : ".")}{typeof(T).Name}";
        }
    }
}
