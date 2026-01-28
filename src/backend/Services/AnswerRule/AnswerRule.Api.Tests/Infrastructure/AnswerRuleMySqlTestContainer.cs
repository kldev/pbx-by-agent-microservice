using AnswerRule.Data;
using App.Shared.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AnswerRule.Api.Tests.Infrastructure;

public class AnswerRuleMySqlTestContainer : MySqlTestContainerBase
{
    protected override async Task InitializeDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<AnswerRuleDbContext>()
            .UseMySQL(ConnectionString)
            .Options;

        await using var context = new AnswerRuleDbContext(options);

        // Apply migrations
        await context.Database.MigrateAsync();

        // Seed test data
        AnswerRuleTestDataSeeder.SeedTestData(context);
        await context.SaveChangesAsync();
    }
}
