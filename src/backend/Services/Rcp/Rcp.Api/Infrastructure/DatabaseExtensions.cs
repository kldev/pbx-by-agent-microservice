using Rcp.Data;
using Microsoft.EntityFrameworkCore;

namespace Rcp.Api.Infrastructure;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = services.GetRequiredService<RcpDbContext>();

            // Apply migrations
            await ApplyMigrationsAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization");
            throw;
        }
    }

    private static async Task ApplyMigrationsAsync(RcpDbContext context, ILogger logger)
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully");
        }
        else
        {
            logger.LogInformation("Database is up to date, no migrations to apply");
        }
    }
}
