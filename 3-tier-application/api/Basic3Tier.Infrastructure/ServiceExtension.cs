using Basic3Tier.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Basic3Tier.Infrastructure;

public static class ServiceExtension
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserMapperProfiles).Assembly);

        services.AddScoped<IUserService, UserService>();
    }
}