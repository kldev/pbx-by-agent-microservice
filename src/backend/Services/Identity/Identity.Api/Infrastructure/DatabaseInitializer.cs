using App.Shared.Web;
using Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Api.Infrastructure;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = services.GetRequiredService<IdentityDbContext>();
            var seedSettings = services.GetRequiredService<IOptions<SeedSettings>>().Value;

            await ApplyMigrationsAsync(context, logger);

            if (seedSettings.RunOnStartup)
            {
                await SeedDataAsync(context, services, seedSettings, logger);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization");
            throw;
        }
    }

    private static async Task ApplyMigrationsAsync(IdentityDbContext context, ILogger logger)
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        var migrations = pendingMigrations as string[] ?? pendingMigrations.ToArray();

        if (migrations.Length > 0)
        {
            logger.LogInformation("Applying {Count} pending migrations...", migrations.Length);
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully");
        }
        else
        {
            logger.LogInformation("Database is up to date, no migrations to apply");
        }
    }

    private static async Task SeedDataAsync(
        IdentityDbContext context,
        IServiceProvider services,
        SeedSettings seedSettings,
        ILogger logger)
    {
        var isDatabaseEmpty = !await context.Employees.AnyAsync();
        var seedService = services.GetRequiredService<ISeedService>();

        await seedService.SeedDictionariesAsync();

        if (isDatabaseEmpty && seedSettings.IncludeShowcaseData)
        {
            logger.LogInformation("Fresh database detected, seeding showcase data...");
            await seedService.SeedShowcaseDataAsync();
        }
        else if (seedSettings.IncludeShowcaseData && !isDatabaseEmpty)
        {
            logger.LogInformation("Database not empty, skipping showcase seed (use ResetShowcaseData to force)");
        }

        logger.LogInformation("Seed completed (Showcase: {Showcase}, DatabaseWasEmpty: {Empty})",
            seedSettings.IncludeShowcaseData, isDatabaseEmpty);
    }
}
