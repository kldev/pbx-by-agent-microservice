using App.Shared.Tests.Infrastructure;
using Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Tests.Infrastructure;

/// <summary>
/// MySQL test container for Identity service.
/// Handles database migrations and test data seeding.
/// </summary>
public class IdentityMySqlTestContainer : MySqlTestContainerBase
{
    protected override async Task InitializeDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseMySQL(ConnectionString)
            .Options;

        await using var context = new IdentityDbContext(options);

        // Apply migrations (not EnsureCreatedAsync - to be compatible with app startup)
        await context.Database.MigrateAsync();

        // Seed test data
        IdentityTestDataSeeder.SeedTestData(context);
        await context.SaveChangesAsync();
    }
}
