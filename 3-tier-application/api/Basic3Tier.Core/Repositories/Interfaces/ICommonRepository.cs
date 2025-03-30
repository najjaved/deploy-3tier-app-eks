using System.Linq.Expressions;

namespace Basic3Tier.Core;

public interface ICommonRepository<TEntity> where TEntity : CommonEntity
{
    Task<IQueryable<TEntity>> GetAllAsync();

    Task<IQueryable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> expression);

    Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> expression);

    Task<int> GetCountAsync(Expression<Func<TEntity, bool>> expression);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression);

    Task<IQueryable<TEntity>> AsQueryableAsync();

    bool Add(TEntity entity);

    bool AddRange(IEnumerable<TEntity> entities);

    bool Update(TEntity entity);

    bool UpdateRange(IEnumerable<TEntity> entities);

    bool Remove(TEntity entity);

    bool RemoveRange(IEnumerable<TEntity> entities);

    Task SaveChangesAsync();
}