using App.Shared.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using RateService.Data;


namespace RateService.Api.Tests.Infrastructure;

public class RateServiceMySqlTestContainer : MySqlTestContainerBase
{
    protected override async Task InitializeDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<RateServiceDbContext>()
            .UseMySQL(ConnectionString)
            .Options;

        await using var context = new RateServiceDbContext(options);

        // Apply migrations (not EnsureCreatedAsync - to be compatible with app startup)
        await context.Database.MigrateAsync();

        // Seed test data
        // RateServiceTestDataSeeder.SeedTestData(context);
        await context.SaveChangesAsync();
    }
}
