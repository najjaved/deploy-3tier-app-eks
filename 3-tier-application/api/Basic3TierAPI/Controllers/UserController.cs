using Basic3Tier.Core;
using Basic3Tier.Infrastructure;
using Basic3Tier.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Basic3TierAPI.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : CommonRestController<UserQueryParameters, UserDtoRequest, User, IUserRepository>
{
    public UserController(
        ILogger<User> logger,
        IUserService service)
        : base(logger, service)
    {
        // Nothing to do here
    }
}