using Grpc.Domain.Model;
using System.Linq.Expressions;

namespace Grpc.Infrastructure.Repository
{
    public interface IRepository<T> where T : EntityBase
    {
        Task<bool> Delete(T entity);

        Task<bool> Delete(object id);

        Task<object> ExecuteRawSql(string query, params object[] parameters);

        Task<IEnumerable<T>> Get(int page = 1,
                                    int pageSize = 25,
                                    Expression<Func<T, bool>>? filter = default,
                                    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = default,
                                    string? includeProperties = default);

        Task<T> GetById(object id);

        Task<bool> Insert(T entity);

        Task<bool> Update(T entity);
    }
}