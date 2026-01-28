using App.Shared.Tests.Infrastructure;
using Rcp.Data;
using Microsoft.EntityFrameworkCore;

namespace Rcp.Api.Tests.Infrastructure;

/// <summary>
/// MySQL test container for RCP service.
/// Handles database migrations and test data seeding.
/// </summary>
public class RcpMySqlTestContainer : MySqlTestContainerBase
{
    protected override async Task InitializeDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<RcpDbContext>()
            .UseMySQL(ConnectionString)
            .Options;

        await using var context = new RcpDbContext(options);

        // Apply migrations (not EnsureCreatedAsync - to be compatible with app startup)
        await context.Database.MigrateAsync();

        // Seed test data
        RcpTestDataSeeder.SeedTestData(context);
        await context.SaveChangesAsync();
    }
}
