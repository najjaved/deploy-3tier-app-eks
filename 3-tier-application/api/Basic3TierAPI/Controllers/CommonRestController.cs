using Basic3Tier.Core;
using Basic3Tier.Infrastructure;
using Basic3Tier.Infrastructure.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Basic3TierAPI.Controllers;

public abstract class CommonRestController<TParameters, TRequest, TModel, TRepository>
    : ControllerBase

    where TParameters : QueryParameters
    where TRequest : CommonDtoRequest
    where TModel : CommonEntity
    where TRepository : ICommonRepository<TModel>
{

    protected readonly ICommonService<TParameters, TRequest, TModel, TRepository> _service;
    protected readonly ILogger _logger;

    public CommonRestController(ILogger logger, ICommonService<TParameters, TRequest, TModel, TRepository> service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public virtual async Task<IActionResult> Get([FromQuery] TParameters parameters)
    {
        try
        {
            var response = await _service.GetEntitiesAsync(parameters);

            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Entity: Leaving Controller...");
            return Problem("Some error occured while fetching records");
        }
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> Get(int id, [FromQuery] TParameters parameters)
    {
        if (id < 1)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response = await _service.GetEntityByIdAsync(id, parameters);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
        catch (ResourceNotFoundException exception)
        {
            _logger.LogError(exception, "Entity: Leaving Controller...");
            return NotFound(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Entity: Leaving Controller...");
            return Problem("Some error occured while fetching records");
        }
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post([FromBody] TRequest request)
    {
        if (request == null)
        {
            return BadRequest(base.ModelState);
        }
        try
        {
            var response = await _service.InsertEntityAsync(request);
            if (!base.ModelState.IsValid)
            {
                return BadRequest(base.ModelState);
            }
            if (response == null)
            {
                return Problem("Some error occured while saving the record");
            }

            return CreatedAtAction(nameof(Post), new { response.Id }, response);
        }
        catch (ResourceNotFoundException exception)
        {
            _logger.LogError(exception, "Entity: Leaving Controller...");
            return NotFound(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Entity: Leaving Controller...");
            return Problem("Some error occured while saving the record");
        }
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Put(int id, [FromBody] TRequest request)
    {
        if (id < 1 || request == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var response = await _service.UpdateEntityAsync(id, request);
            if (response == null)
            {
                return Problem("Some error occured while updating the record");
            }
            return Ok(response);
        }
        catch (ResourceNotFoundException exception)
        {
            _logger.LogError(exception, "Entity: Leaving Controller...");
            return NotFound(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Entity: Leaving Controller...");
            return Problem("Some error occured while updating the record");
        }
    }

    [HttpPatch("{id}")]
    public virtual async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<TRequest> request)
    {
        if (id < 1 || request == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var response = await _service.UpdateEntityAsync(id, request);
            if (response == null)
            {
                return Problem("Some error occured while updating the record");
            }
            return Ok(response);
        }
        catch (ResourceNotFoundException exception)
        {
            _logger.LogError(exception, "Entity: Leaving Controller...");
            return NotFound(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Entity: Leaving Controller...");
            return Problem("Some error occured while updating the record");
        }
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(int id)
    {
        if (id < 1)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var status = await _service.DeleteEntityAsync(id);
            if (status)
            {
                return Ok();
            }
            return Problem("Some error occured while deleting the record");
        }
        catch (ResourceNotFoundException exception)
        {
            _logger.LogError(exception, "Entity: Leaving Controller...");
            return NotFound(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Entity: Leaving Controller...");
            return Problem("Some error occured while deleting the record");
        }
    }
}