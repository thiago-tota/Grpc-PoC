using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Grpc.Domain.Model;

namespace Grpc.Infrastructure.Repository
{
    public abstract class RepositoryEfBase<TEntity>
        : IRepository<TEntity> where TEntity : EntityBase
    {
        protected DbContext Context { get; }
        protected DbSet<TEntity> DbSet { get; }

        protected RepositoryEfBase(DbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public virtual async Task<bool> Delete(TEntity entity)
        {
            DbSet.Remove(entity);
            Context.Entry(entity).State = EntityState.Deleted;

            return await SaveChanges().ConfigureAwait(false);
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

        public virtual async Task<List<TEntity>> Get(int page = 1,
            int pageSize = 25,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
        {
            IQueryable<TEntity> query = DbSet;

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

            query = orderBy != null
                ? orderBy(query).Skip((page - 1) * pageSize).Take(pageSize)
                : query.Take(pageSize);

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity> GetById(object id)
        {
            return await DbSet.FindAsync(id).ConfigureAwait(false);
        }

        public virtual async Task<bool> Insert(TEntity entity)
        {
            DbSet.Add(entity);
            return await SaveChanges().ConfigureAwait(false);
        }

        public virtual async Task<bool> Update(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            return await SaveChanges().ConfigureAwait(false);
        }

        public virtual async Task<bool> SaveChanges()
        {
            return await Context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }
    }
}
