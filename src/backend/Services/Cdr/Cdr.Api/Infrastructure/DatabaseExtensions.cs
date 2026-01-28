using Microsoft.EntityFrameworkCore;
using CdrService.Api.Seed;
using CdrService.Data;

namespace CdrService.Api.Infrastructure;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CdrDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // Migracje
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                await context.Database.MigrateAsync();
            }
            else
            {
                // Ensure database is created if no migrations
                await context.Database.EnsureCreatedAsync();
            }

            // Seed
            var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
            await seedService.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error initializing database");
            throw;
        }
    }
}
