using AnswerRule.Api.Seed;
using AnswerRule.Data;
using Microsoft.EntityFrameworkCore;

namespace AnswerRule.Api.Infrastructure;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AnswerRuleDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // Migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                await context.Database.MigrateAsync();
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
