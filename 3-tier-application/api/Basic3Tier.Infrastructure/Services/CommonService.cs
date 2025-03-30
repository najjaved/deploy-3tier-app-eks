using AutoMapper;
using Basic3Tier.Core;
using Basic3Tier.Infrastructure.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Transactions;

namespace Basic3Tier.Infrastructure;

public abstract class CommonService<TParameters, TRequest, TEntity, TRepository>
    : ICommonService<TParameters, TRequest, TEntity, TRepository>
    where TParameters : QueryParameters, new()
    where TRequest : CommonDtoRequest
    where TEntity : CommonEntity
    where TRepository : ICommonRepository<TEntity>
{
    protected readonly ILogger<TEntity> _logger;
    protected readonly IMapper _mapper;
    protected readonly TRepository _repository;
    protected readonly string PrimaryKeyName;

    public CommonService(
      ILogger<TEntity> logger,
      IMapper mapper,
      TRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    public abstract void ProcessEntityUpdate(TEntity dbEntity, TEntity requestEntity);

    public virtual void AddCommonExeptionData(Exception ex)
    {
        ex.Data.Add(nameof(TParameters), typeof(TParameters));
        ex.Data.Add(nameof(TRequest), typeof(TRequest));
        ex.Data.Add(nameof(TEntity), typeof(TEntity));
        ex.Data.Add(nameof(TRepository), typeof(TRepository));
    }

    public void LogPrimaryKeyMissingInfo(string pk)
    {
        _logger.LogWarning(message: "Primary Key '{pk}' is missing in database", pk);
    }

    public void LogKeyNotMatchingPayload(string key, string payloadValue)
    {
        _logger.LogWarning("Key '{pk}' does not match payload value '{value}'", key, payloadValue);
    }

    public abstract IQueryable<TEntity> AddFilters(IQueryable<TEntity> query, TParameters parameters);

    public virtual async Task<PaginationResult<TRequest>> GetEntitiesAsync(TParameters parameters)
    {
        try
        {
            var query = await _repository.AsQueryableAsync();
            query = AddFilters(query, parameters);

            int totalRecords = query.Count();

            int skip = 0;
            int pageSize = parameters.PageSize < 1 ? 10 : parameters.PageSize;
            int pageNo = parameters.PageNo < 1 ? 1 : parameters.PageNo;
            if (parameters.PageSize == 0)
            {
                pageSize = totalRecords;
                pageNo = 1;
            }
            else if (pageNo > 1)
            {
                skip = (pageNo - 1) * pageSize;
            }

            if (string.IsNullOrWhiteSpace(parameters.OrderBy))
            {
                query = query.OrderBy(o => o.Id);
            }
            else
            {
                var orderByClauses = parameters.OrderBy
                    .Split(',')
                    .Select(orderBy => orderBy.Trim())
                    .ToList();

                foreach (var orderByClause in orderByClauses)
                {
                    if (orderByClause.EndsWith(" DESC"))
                    {
                        var propertyName = orderByClause.Replace(" DESC", "");
                        query = query.OrderByDescending(o => EF.Property<object>(o, propertyName));
                    }
                    else
                    {
                        query = query.OrderBy(o => EF.Property<object>(o, orderByClause));
                    }
                }
            }

            List<TEntity> results = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var data = _mapper.Map<List<TRequest>>(results);

            return new PaginationResult<TRequest>
            {
                Data = data,
                CurrentPage = pageNo,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }
        catch (Exception ex)
        {
            AddCommonExeptionData(ex);
            ex.Data.Add("parameters", parameters.ToJsonString());
            _logger.LogError(ex, "Entity: Unable to get records");
            return null;
        }
    }

    protected virtual async Task<TEntity> GetEntityByIdAsync(int id)
    {
        var records = await _repository.GetWhereAsync(e => e.Id == id);
        var val = records.FirstOrDefault();
        if (val == null)
        {
            LogPrimaryKeyMissingInfo(id.ToString());
            return null;
        }
        return val;
    }


    public virtual async Task<TRequest> GetEntityByIdAsync(int id, TParameters parameters)
    {
        var val = await GetEntityByIdAsync(id);
        if (val == null)
        {
            LogPrimaryKeyMissingInfo(id.ToString());
            return null;
        }
        return _mapper.Map<TRequest>(val);
    }

    public virtual async Task<TRequest> UpdateEntityAsync(int id, TRequest requestDto)
    {
        try
        {
            if (id != requestDto.Id)
            {
                LogKeyNotMatchingPayload(id.ToString(), requestDto.Id.ToString());
                return null;
            }

            var val = await GetEntityByIdAsync(id) ?? throw new ResourceNotFoundException(nameof(TEntity), "Id", id);

            TEntity requestEntity = _mapper.Map<TEntity>(requestDto);
            ProcessEntityUpdate(val, requestEntity);
            return await UpsertEntity(val, requestDto);
        }
        catch (Exception ex)
        {
            AddCommonExeptionData(ex);
            ex.Data.Add("id", id);
            ex.Data.Add("requestDto", requestDto.ToJsonString());
            _logger.LogError(ex, "Entity: Unable to update record");
        }

        return null;
    }

    public virtual async Task<TRequest> UpdateEntityAsync(int id, JsonPatchDocument<TRequest> patchDto)
    {
        try
        {
            var val = await GetEntityByIdAsync(id);
            TRequest val2 = _mapper.Map<TRequest>(val);
            patchDto.ApplyTo(val2);
            _mapper.Map(val2, val);
            return await UpsertEntity(val, val2);
        }
        catch (Exception ex)
        {
            AddCommonExeptionData(ex);
            ex.Data.Add("id", id);
            ex.Data.Add("requestDto", patchDto);
            _logger.LogError(ex, "Entity: Unable to update record");
        }

        return null;
    }

    public virtual async Task<TRequest> UpsertEntity(TEntity dbEntity, TRequest requestDto, bool isUpdate = true)
    {
        try
        {
            bool resoponse;
            if (isUpdate)
            {
                dbEntity.UpdatedOn = DateTime.UtcNow;
                resoponse = _repository.Update(dbEntity);
            }
            else
            {
                dbEntity.UpdatedOn = null;
                dbEntity.CreatedOn = DateTime.UtcNow;
                resoponse = _repository.Add(dbEntity);
            }

            await _repository.SaveChangesAsync();
            if (!resoponse)
            {
                return null;
            }

            TParameters parameters = new()
            {
            };
            return await GetEntityByIdAsync(dbEntity.Id, parameters);
        }
        catch (Exception ex)
        {
            AddCommonExeptionData(ex);
            ex.Data.Add("id", dbEntity.Id);
            ex.Data.Add("requestDto", requestDto.ToJsonString());
            ex.Data.Add("isUpdate", isUpdate);
            _logger.LogError(ex, "Entity: Unable to upsert record");
        }

        return null;
    }

    public virtual async Task<bool> DeleteEntityAsync(int id)
    {
        try
        {
            TEntity entity = await GetEntityByIdAsync(id) ?? throw new ResourceNotFoundException(nameof(TEntity), "Id", id);

            var response = _repository.Remove(entity);
            _logger.LogDebug("Entity record deleted for {id}", id);
            await _repository.SaveChangesAsync();
            return response;
        }
        catch (Exception ex)
        {
            AddCommonExeptionData(ex);
            ex.Data.Add("id", id);
            _logger.LogError(ex, "Entity: Unable to delete record");
            return false;
        }
    }

    public virtual async Task<TRequest> InsertEntityAsync(TRequest requestDto)
    {
        try
        {
            TEntity requestEntity = _mapper.Map<TEntity>(requestDto);
            if (await CheckIfExistsAsync(e => e.Id == requestEntity.Id))
            {
                return null;
            }

            return await UpsertEntity(requestEntity, requestDto, isUpdate: false);
        }
        catch (Exception ex)
        {
            AddCommonExeptionData(ex);
            ex.Data.Add("requestDto", requestDto.ToJsonString());
            _logger.LogError(ex, "Entity: Unable to insert record");
        }

        return null;
    }

    public async virtual Task<bool> BulkInsertAsync(List<TRequest> records)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                var entities = _mapper.Map<List<TEntity>>(records);
                foreach (var entity in entities)
                {
                    entity.Id = 0;
                    entity.CreatedOn = DateTime.UtcNow;
                    entity.UpdatedOn = null;
                }
                var result = _repository.AddRange(entities);
                if (result)
                {
                    await _repository.SaveChangesAsync();
                    scope.Complete();
                }
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                AddCommonExeptionData(ex);
                ex.Data.Add("recordsCount", records.Count);
                _logger.LogError(ex, "Entity: Unable to bulk insert records");
            }
        }
        return await Task.FromResult(false);
    }

    public abstract Task<bool> CheckIfDuplicateAsync(TEntity record);

    public virtual Task<bool> CheckIfExistsAsync(Expression<Func<TEntity, bool>> expression)
    {
        return _repository.AnyAsync(expression);
    }
}
