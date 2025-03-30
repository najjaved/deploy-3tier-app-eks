using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Basic3Tier.Core;

public abstract class CommonRepository<TEntity> : ICommonRepository<TEntity> where TEntity : CommonEntity
{

    private readonly ILogger<CommonRepository<TEntity>> _logger;
    public Basic3TierDbContext DbContext { get; private set; }

    public CommonRepository(
       ILogger<CommonRepository<TEntity>> logger,
       Basic3TierDbContext dbContext)
    {
        _logger = logger;
        DbContext = dbContext;
    }

    public virtual async Task<IQueryable<TEntity>> GetAllAsync()
    {
        IQueryable<TEntity> queryable = DbContext.Set<TEntity>().AsNoTracking().AsQueryable();
        return await Task.FromResult(queryable);
    }

    public virtual async Task<IQueryable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> expression)
    {
        IQueryable<TEntity> queryable = (from r in DbContext.Set<TEntity>()
                                         select r).Where(expression).AsNoTracking().AsQueryable();
        return await Task.FromResult(queryable);
    }

    public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> expression)
    {
        TEntity response = await (from r in DbContext.Set<TEntity>()
                                  select r).Where(expression).AsNoTracking().FirstOrDefaultAsync();
        return response;
    }

    public virtual bool Add(TEntity entity)
    {
        if (entity == null)
        {
            return false;
        }

        return DbContext.Set<TEntity>().Add(entity) != null;
    }

    public virtual bool AddRange(IEnumerable<TEntity> entities)
    {
        if (entities == null)
        {
            return false;
        }

        DbContext.Set<TEntity>().AddRange(entities);

        return true;
    }

    public virtual bool Update(TEntity entity)
    {
        if (entity == null)
        {
            return false;
        }

        return DbContext.Set<TEntity>().Update(entity) != null;
    }

    public virtual bool UpdateRange(IEnumerable<TEntity> entities)
    {
        if (entities == null)
        {
            return false;
        }

        DbContext.Set<TEntity>().UpdateRange(entities);
        return true;
    }

    public virtual bool Remove(TEntity entity)
    {
        if (entity == null)
        {
            return false;
        }

        return DbContext.Set<TEntity>().Remove(entity) != null;
    }

    public virtual bool RemoveRange(IEnumerable<TEntity> entities)
    {
        if (entities == null)
        {
            return false;
        }

        DbContext.Set<TEntity>().RemoveRange(entities); ;
        return true;
    }

    public virtual async Task<IQueryable<TEntity>> AsQueryableAsync()
    {
        IQueryable<TEntity> queryable = DbContext.Set<TEntity>().AsNoTracking().AsQueryable();
        return await Task.FromResult(queryable);
    }

    public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> expression)
    {
        var count = DbContext.Set<TEntity>().Count(expression);
        return await Task.FromResult(count);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression)
    {
        var status = DbContext.Set<TEntity>().Any(expression);
        return await Task.FromResult(status);
    }

    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}
