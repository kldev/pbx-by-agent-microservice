using FinCosts.Api.Seed;
using FinCosts.Data;
using Microsoft.EntityFrameworkCore;

namespace FinCosts.Api.Infrastructure;


public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = services.GetRequiredService<FinCostsDbContext>();

            // Apply migrations
            await ApplyMigrationsAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization");
            throw;
        }
        
        // Seed
        try
        {
            var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
            await seedService.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during seed database");
            throw;
        }

    }

    private static async Task ApplyMigrationsAsync(FinCostsDbContext context, ILogger logger)
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        var migrations = pendingMigrations as string[] ?? pendingMigrations.ToArray();
        if (migrations.Any())
        {
            logger.LogInformation("Applying {Count} pending migrations...", migrations.Count());
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully");
        }
        else
        {
            logger.LogInformation("Database is up to date, no migrations to apply");
        }
    }
}
