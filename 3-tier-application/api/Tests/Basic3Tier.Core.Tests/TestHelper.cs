using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Basic3Tier.Core.Tests;

public class TestHelper : IDisposable
{
    private readonly Basic3TierDbContext _dbContext;
    private readonly Mock<ILogger<UserRepository>> moqLoggerObject;

    public TestHelper()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<Basic3TierDbContext>()
            .UseInMemoryDatabase(databaseName: "basic3tier")
            .UseInternalServiceProvider(serviceProvider);

        _dbContext = new Basic3TierDbContext(builder.Options);

        moqLoggerObject = new Mock<ILogger<UserRepository>>();

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }

    public IUserRepository GetInMemoryUserRepository()
    {
        return new UserRepository(moqLoggerObject.Object, _dbContext);
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}
