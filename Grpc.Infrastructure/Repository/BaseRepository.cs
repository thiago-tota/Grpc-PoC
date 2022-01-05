using System.Data.Entity;
using System.Linq.Expressions;

namespace Grpc.Infrastructure.Repository
{
    public abstract class BaseRepository<T>
        : IRepository<T> where T : class
    {
        protected DbContext Context { get; }
        protected DbSet<T> DbSet { get; }

        public BaseRepository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public virtual async Task<bool> Delete(T entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            return await Task.FromResult(DbSet.Remove(entity) == default);
        }

        public virtual async Task<bool> Delete(object id)
        {
            var entity = await GetById(id).ConfigureAwait(false);
            return await Delete(entity).ConfigureAwait(false);
        }

        public virtual async Task<object> ExecuteRawSql(string query, params object[] parameters)
        {
            return await DbSet.SqlQuery(query, parameters).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> Get(int page = 1,
                                                        int pageSize = 25,
                                                        Expression<Func<T, bool>>? filter = null,
                                                        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                        string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query).Skip((page - 1) * pageSize).Take(pageSize);
            }
            else
            {
                query = query.Take(pageSize);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<T> GetById(object id)
        {
            return await DbSet.FindAsync(id).ConfigureAwait(false);
        }

        public virtual async Task<bool> Insert(T entity)
        {
            return await Task.FromResult(DbSet.Add(entity) == default);
        }

        public virtual async Task<bool> Update(T entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            return await Task.FromResult(Context.Entry(entity).State == EntityState.Modified);
        }
    }
}
