using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Basic3Tier.Core;

public static class ServiceExtension
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public static void AddDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<Basic3TierDbContext>(options =>
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.UseNpgsql(connectionString, o => o.MigrationsAssembly(typeof(ServiceExtension).Assembly.FullName));
        });
    }

    public static void RunMigration(this IServiceProvider service)
    {
        using IServiceScope scope = service.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        try
        {
            Basic3TierDbContext dbContext = services.GetRequiredService<Basic3TierDbContext>();
            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while applying migrations: " + ex.Message);
        }
    }
}
