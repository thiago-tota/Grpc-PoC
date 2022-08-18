using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Domain.Model;
using Grpc.Infrastructure.SqlServer;
using Microsoft.Data.SqlClient;
using static Dapper.SqlMapper;

namespace Grpc.Infrastructure.Repository
{
    public abstract class RepositoryDaoBase<TEntity> : IRepository<TEntity> where TEntity : EntityBase, new()
    {
        protected SqlDatabase _sqlDatabase;

        public RepositoryDaoBase(string connectionString)
        {
            _sqlDatabase = new SqlDatabase(connectionString);
        }

        public virtual async Task<bool> Delete(TEntity entity)
        {
            return await Delete(entity.Id).ConfigureAwait(false);
        }

        public virtual async Task<bool> Delete(object id)
        {
            var defaultInstance = new TEntity();
            var query = $"DELETE FROM {GetTableName(defaultInstance)} WHERE {GetPropertyKey(defaultInstance)} = @Id";

            int result;
            try
            {
                _sqlDatabase.Connect();
                var sqlCommand = new SqlCommand(query, _sqlDatabase.SqlConnection);
                sqlCommand.Parameters.Add(id);
                result = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

            }
            catch
            {
                throw;
            }
            finally
            {
                _sqlDatabase.Disconnect();
            }

            return result > 0;
        }

        public virtual async Task<object> ExecuteRawSql(string query, params object[] parameters)
        {
            var result = default(object);

            try
            {
                _sqlDatabase.Connect();
                var sqlCommand = new SqlCommand(query, _sqlDatabase.SqlConnection);
                AddCommandParameters(sqlCommand, parameters);

                if (query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    await using var dataReader = await sqlCommand.ExecuteReaderAsync().ConfigureAwait(false);
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
                _sqlDatabase.Disconnect();
            }

            return result;
        }

        public virtual async Task<List<TEntity>> Get(int page = 1,
            int pageSize = 25,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
        {
            var defaultInstance = new TEntity();

            var fields = $"{GetPropertyKey(defaultInstance)}, {string.Join(", ", GetPropertyNames(defaultInstance))}";
  
            var query = $"SELECT TOP {pageSize} {fields} FROM {GetTableName(defaultInstance)}";
            
            var result = new List<TEntity>();

            try
            {
                _sqlDatabase.Connect();
                var sqlCommand = new SqlCommand(query, _sqlDatabase.SqlConnection);
                await using var dataReader = await sqlCommand.ExecuteReaderAsync().ConfigureAwait(false);

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
                _sqlDatabase.Disconnect();
            }

            return result;
        }

        public virtual async Task<TEntity> GetById(object id)
        {
            var result = await Get(pageSize: 1, filter: f => f.Id == id);
            return result.First();
        }

        public virtual async Task<bool> Insert(TEntity entity)
        {
            var properties = GetPropertyNames(entity);
            var fields = string.Join(", ", properties);
            var parameters = string.Join(", @", properties).Insert(0, "@");

            var query = $"INSERT INTO {GetTableName(entity)} ({fields}) VALUES ({parameters})";

            int result = 0;

            try
            {
                _sqlDatabase.Connect();
                var sqlCommand = new SqlCommand(query, _sqlDatabase.SqlConnection);
                AddCommandParameters(sqlCommand, entity);

                result = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
            finally
            {
                _sqlDatabase.Disconnect();
            }

            return result > 0;
        }

        public virtual async Task<bool> Update(TEntity entity)
        {
            var properties = GetPropertyNames(entity);
            var fields = string.Join(", ", properties, " = @", properties);

            var query = $"UPDATE {GetTableName(entity)} SET {fields}";

            int result = 0;
            try
            {
                _sqlDatabase.Connect();
                var sqlCommand = new SqlCommand(query, _sqlDatabase.SqlConnection);
                AddCommandParameters(sqlCommand, entity);

                result = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
            finally
            {
                _sqlDatabase.Disconnect();
            }

            return result > 0;
        }

        protected List<PropertyInfo> GetProperties(TEntity entity)
        {
            return entity.GetType().GetProperties()
                .Where(item =>
                    item.CustomAttributes.All(f =>
                                                f.AttributeType != typeof(NotMappedAttribute) &&
                                                f.AttributeType != typeof(KeyAttribute))
                    ).ToList();
        }

        protected List<string> GetPropertyNames(TEntity entity)
        {
            return entity.GetType().GetProperties()
                .Where(item =>
                    item.CustomAttributes.All(f =>
                        f.AttributeType != typeof(NotMappedAttribute) &&
                        f.AttributeType != typeof(KeyAttribute))
                ).Select(p => p.Name).ToList();
        }

        protected string GetPropertyKey(TEntity entity)
        {
            return entity.GetType().GetProperties().First(item =>
                            item.CustomAttributes.Any(f => f.AttributeType == typeof(KeyAttribute))).Name;
        }

        private TEntity GetRecord(IDataReader dataReader)
        {
            var entity = new TEntity();

            foreach (var item in GetProperties(entity))
            {
                if (item.CustomAttributes.All(f => f.AttributeType != typeof(NotMappedAttribute)) && dataReader[item.Name] != DBNull.Value)
                    item.SetValue(entity, dataReader[item.Name]);
            }

            return entity;
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

        private void AddCommandParameters(SqlCommand sqlCommand, TEntity entity)
        {
            foreach (var item in GetProperties(entity))
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

        protected string GetTableName(TEntity entity)
        {
            return $"{entity.Namespace}{(entity.Namespace == default ? "" : ".")}{typeof(TEntity).Name}";
        }
    }
}
