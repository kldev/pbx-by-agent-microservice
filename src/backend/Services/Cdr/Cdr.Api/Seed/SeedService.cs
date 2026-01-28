using Microsoft.EntityFrameworkCore;
using CdrService.Data;

namespace CdrService.Api.Seed;

public class SeedService : ISeedService
{
    private readonly CdrDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SeedService> _logger;

    public SeedService(CdrDbContext context, IConfiguration configuration, ILogger<SeedService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        var runOnStartup = _configuration.GetValue<bool>("Seed:RunOnStartup");
        if (!runOnStartup)
        {
            _logger.LogInformation("Seeding is disabled");
            return;
        }

        _logger.LogInformation("Starting database seeding...");

        // Seed dictionaries (always)
        await SeedCallStatusesAsync();
        await SeedTerminationCausesAsync();
        await SeedCallDirectionsAsync();

        // Seed showcase data (optional)
        var includeShowcase = _configuration.GetValue<bool>("Seed:IncludeShowcaseData");
        if (includeShowcase)
        {
            await SeedShowcaseCallRecordsAsync();
        }

        _logger.LogInformation("Database seeding completed");
    }

    private async Task SeedCallStatusesAsync()
    {
        if (await _context.CallStatuses.AnyAsync())
        {
            _logger.LogInformation("CallStatuses already seeded");
            return;
        }

        var statuses = ShowcaseData.GetCallStatuses();
        _context.CallStatuses.AddRange(statuses);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} call statuses", statuses.Count);
    }

    private async Task SeedTerminationCausesAsync()
    {
        if (await _context.TerminationCauses.AnyAsync())
        {
            _logger.LogInformation("TerminationCauses already seeded");
            return;
        }

        var causes = ShowcaseData.GetTerminationCauses();
        _context.TerminationCauses.AddRange(causes);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} termination causes", causes.Count);
    }

    private async Task SeedCallDirectionsAsync()
    {
        if (await _context.CallDirections.AnyAsync())
        {
            _logger.LogInformation("CallDirections already seeded");
            return;
        }

        var directions = ShowcaseData.GetCallDirections();
        _context.CallDirections.AddRange(directions);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} call directions", directions.Count);
    }

    private async Task SeedShowcaseCallRecordsAsync()
    {
        if (await _context.CallRecords.AnyAsync())
        {
            _logger.LogInformation("CallRecords already seeded");
            return;
        }

        var records = ShowcaseData.GetSampleCallRecords();
        _context.CallRecords.AddRange(records);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} sample call records", records.Count);
    }
}
