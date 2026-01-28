using System.Reflection;
using DataSource.Data;
using Microsoft.EntityFrameworkCore;

namespace DataSource.Api.Infrastructure;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataSourceDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
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

            // Create/update SQL views from embedded resources
            await CreateViewsAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }

    private static async Task CreateViewsAsync(DataSourceDbContext context, ILogger logger)
    {
        var assembly = typeof(DataSourceDbContext).Assembly;
        var resourcePrefix = "DataSource.Data.Resources.Views.";

        var viewResources = assembly.GetManifestResourceNames()
            .Where(r => r.StartsWith(resourcePrefix) && r.EndsWith(".sql"))
            .OrderBy(r => r) // Ensure consistent order
            .ToList();

        logger.LogInformation("Found {Count} SQL view scripts to execute", viewResources.Count);

        foreach (var resourceName in viewResources)
        {
            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    logger.LogWarning("Could not load embedded resource: {Resource}", resourceName);
                    continue;
                }

                using var reader = new StreamReader(stream);
                var sql = await reader.ReadToEndAsync();

                await context.Database.ExecuteSqlRawAsync(sql);
                logger.LogInformation("Executed view script: {Resource}", resourceName.Replace(resourcePrefix, ""));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to execute view script: {Resource}", resourceName);
            }
        }
    }
}
