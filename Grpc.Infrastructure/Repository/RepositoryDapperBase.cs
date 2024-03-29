﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Grpc.Domain.Model;

namespace Grpc.Infrastructure.Repository
{
    /// <summary>
    /// IMPORTANT
    /// RepositoryDaoBase was used as a base class to save some code duplication only because this is an exercise to compare code and performance between DAO, Dapper and EF.
    /// As a comparison in this class there are only the differences between DAO x Dapper
    /// In a real scenario RepositoryDapperBase should be dependent of IRepository<T> and have all necessary code in it.
    /// </summary>
    public abstract class RepositoryDapperBase<T> : RepositoryDaoBase<T> where T : EntityBase, new()
    {
        public RepositoryDapperBase(string connectionString) : base(connectionString)
        {
        }

        public override async Task<bool> Delete(object id)
        {
            var defaultInstance = new T();
            var query = $"DELETE FROM {GetTableName(defaultInstance)} WHERE {GetPropertyKey(defaultInstance)} = @Id";

            int result;
            try
            {
                _sqlDatabase.Connect();
                result = await _sqlDatabase.SqlConnection.ExecuteAsync(query, new { Id = id }).ConfigureAwait(false);
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

        public override async Task<object> ExecuteRawSql(string query, params object[] parameters)
        {
            var result = default(object);

            try
            {
                _sqlDatabase.Connect();

                if (query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    result = await _sqlDatabase.SqlConnection.QueryAsync(query, GetCommandParameters(parameters)).ConfigureAwait(false);
                }
                else
                {
                    result = await _sqlDatabase.SqlConnection.ExecuteAsync(query, GetCommandParameters(parameters)).ConfigureAwait(false);
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

        public override async Task<List<T>> Get(int page = 1,
            int pageSize = 25,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null)
        {
            var defaultInstance = new T();
            var fields = string.Join(", ", GetPropertyNames(defaultInstance));

            var query = $"SELECT TOP {pageSize} {fields} FROM {GetTableName(defaultInstance)}";

            IEnumerable<T> result = default;

            try
            {
                _sqlDatabase.Connect();
                result = await _sqlDatabase.SqlConnection.QueryAsync<T>(query).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
            finally
            {
                _sqlDatabase.Disconnect();
            }

            return result.ToList();
        }

        public override async Task<bool> Insert(T entity)
        {
            var properties = GetPropertyNames(entity);
            var fields = string.Join(", ", properties);
            var parameters = string.Join(", @", properties).Insert(0, "@");

            var query = $"INSERT INTO {GetTableName(entity)} ({fields}) VALUES ({parameters})";

            int result = 0;

            try
            {
                _sqlDatabase.Connect();
                result = await _sqlDatabase.SqlConnection.ExecuteAsync(query, GetCommandParameters(entity)).ConfigureAwait(false);
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

        public override async Task<bool> Update(T entity)
        {
            var properties = GetPropertyNames(entity);
            var fields = string.Join(", ", properties, " = @", properties);

            var query = $"UPDATE {GetTableName(entity)} SET {fields}";

            int result = 0;
            try
            {
                _sqlDatabase.Connect();
                result = await _sqlDatabase.SqlConnection.ExecuteAsync(query, GetCommandParameters(entity)).ConfigureAwait(false);
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

        private DynamicParameters GetCommandParameters(T entity)
        {
            var dynamicParameters = new DynamicParameters();

            foreach (var item in entity.GetType().GetProperties()
                                                     .Where(item =>
                                                         item.CustomAttributes.All(f =>
                                                             f.AttributeType != typeof(NotMappedAttribute) &&
                                                             f.AttributeType != typeof(KeyAttribute))))
            {
                dynamicParameters.Add(item.Name, item.GetValue(entity));
            }

            return dynamicParameters;
        }

        private DynamicParameters GetCommandParameters(params object[] parameters)
        {
            var dynamicParameters = new DynamicParameters();

            foreach (var item in parameters)
            {
                dynamicParameters.Add(item.ToString(), item);
            }

            return dynamicParameters;
        }
    }
}
