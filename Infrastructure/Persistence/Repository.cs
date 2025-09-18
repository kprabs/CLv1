using CoreLib.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CoreLib.Infrastructure.Persistence
{
    public class Repository<TDbContext>(TDbContext dbContext) : IRepository, IRepository<TDbContext> where TDbContext : DbContext
    {
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            CheckNullEntity(entity);
            dbContext.Set<TEntity>().Add(entity);
        }

        public async Task AddAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            CheckNullEntity(entity);
            await dbContext.Set<TEntity>().AddAsync(entity, default).ConfigureAwait(false);
        }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            CheckNullEntity(entities);
            dbContext.Set<TEntity>().AddRange(entities);
        }

        public async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            CheckNullEntity(entities);
            await dbContext.Set<TEntity>().AddRangeAsync(entities, default).ConfigureAwait(false);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            CheckNullEntity(entity);

            dbContext.Set<TEntity>().Update(entity);
        }

        public void UpdateOnCondition<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity> updateAction) where TEntity: class
        {
            CheckNullEntity(updateAction);
            var dbSet  = dbContext.Set<TEntity>();
            var entities = dbSet.Where(predicate).ToList();
            foreach(var entity in entities)
            {
                updateAction(entity);
            }
            dbContext.SaveChanges();
        }

        public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            CheckNullEntity(entities);

            dbContext.Set<TEntity>().UpdateRange(entities);
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            CheckNullEntity(entity);

            dbContext.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            CheckNullEntity(entities);

            dbContext.Set<TEntity>().RemoveRange(entities);
        }

        public IQueryable<TEntity> GetAllResult<TEntity>() where TEntity : class
        {
            return dbContext.Set<TEntity>();
        }
        public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return dbContext.Set<TEntity>().ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>() where TEntity : class
        {
            return await dbContext.Set<TEntity>().ToListAsync();
        }


        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(Func<IQueryable<TEntity>, IQueryable<TEntity>> includes)
            where TEntity : class
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>().AsQueryable();

            return await includes(query).ToListAsync();
        }

        public TEntity? FindById<TEntity>(int id) where TEntity : class
        {
            return dbContext.Set<TEntity>().Find(id);
        }

        public async Task<TEntity?> FindByIdAsync<TEntity>(int id) where TEntity : class
        {
            return await dbContext.FindAsync<TEntity>(id);
        }

        public TEntity? FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return dbContext.Set<TEntity>().FirstOrDefault(predicate);
        }

        public async Task<IEnumerable<TEntity?>> GetAllByWhere<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>().Where(predicate);
            return await query.ToListAsync();
        }

        public async Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return await dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public async Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes) where TEntity : class
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>().Where(predicate);

            return await includes(query).FirstOrDefaultAsync();
        }

        public async Task<TEntity?> SingleOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return await dbContext.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public async Task<TEntity?> SingleOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes) where TEntity : class
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>().Where(predicate);

            return await includes(query).FirstOrDefaultAsync();
        }
        public async Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await dbContext.Set<TEntity>().SingleOrDefaultAsync(predicate);

        }

        public async Task<TEntity?> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes) where TEntity : class
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>().Where(predicate);

            return await includes(query).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> FindWhere<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return await dbContext.Set<TEntity>().Where<TEntity>(predicate).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindWhere<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes)
            where TEntity : class
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>().Where(predicate);

            return await includes(query).ToListAsync();
        }
        public TEntity GetMaxByProperty<TEntity, TProperty>(Func<TEntity, TProperty> selector)
            where TEntity : class
        {
            return dbContext.Set<TEntity>().OrderByDescending(selector).FirstOrDefault();
        }

        public TProperty GetMaxByProperty<TEntity, TProperty>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProperty>> selector)
            where TEntity : class
        {
            return dbContext.Set<TEntity>().Where(predicate).Max(selector);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public int SaveChanges()
        {
            int count = dbContext.SaveChanges();
            return count;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            int count = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return count;
        }

        public async Task<int> SaveChangesAsync()
        {
            int count = await dbContext.SaveChangesAsync().ConfigureAwait(false);
            return count;
        }

        private void CheckNullEntity<TEntity>(TEntity entity)
        {
            if (EqualityComparer<TEntity>.Default.Equals(entity, default))
            {
                throw new ArgumentNullException(nameof(entity));
            }
        }
        public async Task<IEnumerable<TResult>> FindWhere<TEntity, TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selectClause) where TEntity : class
        {
            return await dbContext.Set<TEntity>().Where<TEntity>(predicate).Select<TEntity, TResult>(selectClause).ToListAsync();
        }

        public async Task<TResult?> SingleOrDefaultAsync<TEntity, TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selectClause) where TEntity : class
        {
            return await dbContext.Set<TEntity>().Where(predicate).Select(selectClause).SingleOrDefaultAsync();
        }
    }
}
