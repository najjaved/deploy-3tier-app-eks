using Microsoft.Extensions.Logging;

namespace Basic3Tier.Core;

public class UserRepository : CommonRepository<User>, IUserRepository
{
    public UserRepository(ILogger<UserRepository> logger,
        Basic3TierDbContext dbContext)
        : base(logger, dbContext)
    {
        // nothing to do here
    }
}
