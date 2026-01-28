using AnswerRule.Api.Features.Rules;
using AnswerRule.Api.Seed;
using AnswerRule.Data;
using Microsoft.EntityFrameworkCore;

namespace AnswerRule.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AnswerRuleDbContext>(options =>
        {
            options.UseMySQL(connectionString!);
        });

        // Rules feature
        services.AddScoped<IRuleDataHandler, RuleDataHandler>();
        services.AddScoped<IRuleService, RuleService>();

        // Seed
        services.AddScoped<ISeedService, SeedService>();

        return services;
    }
}
