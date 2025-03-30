using AutoMapper;
using Basic3Tier.Core;
using Basic3Tier.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace Basic3Tier.Infrastructure;

public class UserService : CommonService<UserQueryParameters, UserDtoRequest, User, IUserRepository>, IUserService
{
    public UserService(
        ILogger<User> logger,
        IMapper mapper,
        IUserRepository repository)
        : base(logger, mapper, repository)
    {
        // nothing to do here
    }

    public override IQueryable<User> AddFilters(IQueryable<User> query, UserQueryParameters parameters)
    {
        return query;
    }

    public override async Task<bool> CheckIfDuplicateAsync(User record)
    {
        return await Task.FromResult(false);
    }

    public override void ProcessEntityUpdate(User dbEntity, User requestEntity)
    {
        dbEntity.Age = requestEntity.Age;
        dbEntity.Name = requestEntity.Name;
        dbEntity.Email = requestEntity.Email;
        dbEntity.Address = requestEntity.Address;
        dbEntity.UpdatedOn = DateTime.UtcNow;
    }
}
