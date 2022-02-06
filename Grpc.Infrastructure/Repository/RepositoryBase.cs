using Grpc.Domain.Model;
using System.Linq.Expressions;

namespace Grpc.Infrastructure.Repository
{
    public abstract class RepositoryBase<T>
    : IRepository<T> where T : EntityBase
    {
        public Task<bool> Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(object id)
        {
            throw new NotImplementedException();
        }

        public Task<object> ExecuteRawSql(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> Get(int page = 1,
                                            int pageSize = 25,
                                            Expression<Func<T, bool>>? filter = null,
                                            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                            string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetById(object id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Insert(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
