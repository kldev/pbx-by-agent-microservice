using AnswerRule.Data;
using Microsoft.EntityFrameworkCore;

namespace AnswerRule.Api.Seed;

public class SeedService : ISeedService
{
    private readonly AnswerRuleDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SeedService> _logger;

    public SeedService(
        AnswerRuleDbContext context,
        IConfiguration configuration,
        ILogger<SeedService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        var seedConfig = _configuration.GetSection("Seed");

        if (!seedConfig.GetValue<bool>("RunOnStartup"))
        {
            _logger.LogInformation("Seed disabled in configuration");
            return;
        }

        _logger.LogInformation("Starting database seed...");

        if (seedConfig.GetValue<bool>("IncludeShowcaseData"))
        {
            await SeedRulesAsync();
        }

        _logger.LogInformation("Database seed completed");
    }

    private async Task SeedRulesAsync()
    {
        var rules = ShowcaseData.GetRules();

        foreach (var rule in rules)
        {
            var exists = await _context.AnsweringRules
                .AnyAsync(r => r.Gid == rule.Gid);

            if (!exists)
            {
                _context.AnsweringRules.Add(rule);
                _logger.LogInformation("Seeded rule: {Name}", rule.Name);
            }
        }

        await _context.SaveChangesAsync();
    }
}
