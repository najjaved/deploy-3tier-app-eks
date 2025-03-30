using AutoMapper;
using Basic3Tier.Core;

namespace Basic3Tier.Infrastructure.Models;

public class UserMapperProfiles : Profile
{
    public UserMapperProfiles()
    {
        CreateMap<UserDtoRequest, User>().ReverseMap();
    }
}
