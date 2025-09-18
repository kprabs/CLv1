using System.Linq.Expressions;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IRepository
    {
        Task AddAsync<TEntity>(TEntity entity)
            where TEntity : class;
        void Add<TEntity>(TEntity entity)
            where TEntity : class;
        void AddRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class;
        Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class;
        void Update<TEntity>(TEntity entity)
            where TEntity : class;
        void UpdateRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class;
        void Remove<TEntity>(TEntity entity)
            where TEntity : class;
        void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class;
        IEnumerable<TEntity> GetAll<TEntity>()
            where TEntity : class;
        IQueryable<TEntity> GetAllResult<TEntity>()
            where TEntity : class;
        Task<IEnumerable<TEntity>> GetAllAsync<TEntity>()
            where TEntity : class;
        Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(Func<IQueryable<TEntity>, IQueryable<TEntity>> includes)
            where TEntity : class;
        TEntity? FindById<TEntity>(int id)
            where TEntity : class;
        Task<TEntity?> FindByIdAsync<TEntity>(int id)
            where TEntity : class;
        TEntity? FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;
        Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;
        Task<IEnumerable<TEntity?>> GetAllByWhere<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;
        Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes) where TEntity : class;
        Task<TEntity?> SingleOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;
        Task<TEntity?> SingleOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes) where TEntity : class;
        Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;
        Task<TEntity?> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes) where TEntity : class;
        Task<IEnumerable<TEntity>> FindWhere<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;
        Task<IEnumerable<TEntity>> FindWhere<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes)
            where TEntity : class;
        TEntity GetMaxByProperty<TEntity, TProperty>(Func<TEntity, TProperty> selector)
            where TEntity : class;
        TProperty GetMaxByProperty<TEntity, TProperty>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProperty>> selector)
            where TEntity : class;
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<TResult>> FindWhere<TEntity, TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selectClause)
            where TEntity : class;
        Task<TResult?> SingleOrDefaultAsync<TEntity, TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selectClause)
            where TEntity : class;
        void UpdateOnCondition<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity> updateAction) where TEntity : class;

    }
    public interface IRepository<TDbContext> : IRepository
    {
    }
}
