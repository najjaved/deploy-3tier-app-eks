using Basic3Tier.Core;
using Basic3Tier.Infrastructure.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace Basic3Tier.Infrastructure;

public interface ICommonService<TParameters, TRequest, TEntity, TRepository>
    where TParameters : QueryParameters
    where TRequest : CommonDtoRequest
    where TEntity : CommonEntity
    where TRepository : ICommonRepository<TEntity>
{

    Task<bool> BulkInsertAsync(List<TRequest> records);
    Task<bool> DeleteEntityAsync(int id);
    Task<PaginationResult<TRequest>> GetEntitiesAsync(TParameters parameters);
    Task<TRequest> GetEntityByIdAsync(int id, TParameters parameters);
    Task<TRequest> InsertEntityAsync(TRequest requestDto);
    void ProcessEntityUpdate(TEntity dbEntity, TEntity requestEntity);
    Task<TRequest> UpdateEntityAsync(int id, TRequest request);
    Task<TRequest> UpdateEntityAsync(int id, JsonPatchDocument<TRequest> request);
    Task<bool> CheckIfDuplicateAsync(TEntity record);
    Task<bool> CheckIfExistsAsync(Expression<Func<TEntity, bool>> expression);
}