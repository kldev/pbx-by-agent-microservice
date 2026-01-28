using App.Shared.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using CdrService.Data;

namespace CdrService.Api.Tests.Infrastructure;

public class CdrMySqlTestContainer : MySqlTestContainerBase
{
    protected override async Task InitializeDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<CdrDbContext>()
            .UseMySQL(ConnectionString)
            .Options;

        await using var context = new CdrDbContext(options);

        // Apply migrations (not EnsureCreatedAsync - to be compatible with app startup)
        await context.Database.MigrateAsync();

        await context.SaveChangesAsync();
    }
}
