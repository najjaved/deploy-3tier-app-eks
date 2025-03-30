using Basic3Tier.Core;
using Basic3Tier.Infrastructure.Models;

namespace Basic3Tier.Infrastructure;
public interface IUserService : ICommonService<UserQueryParameters, UserDtoRequest, User, IUserRepository>
{
}
