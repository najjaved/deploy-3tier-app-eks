using Microsoft.EntityFrameworkCore;

namespace Basic3Tier.Core;

public class Basic3TierDbContext : DbContext
{
    public Basic3TierDbContext(DbContextOptions options)
        : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
}
