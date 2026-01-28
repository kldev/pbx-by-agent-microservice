using Microsoft.EntityFrameworkCore;
using RateService.Data;

namespace RateService.Api.Seed;

public class SeedService : ISeedService
{
    private readonly RateServiceDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SeedService> _logger;

    public SeedService(
        RateServiceDbContext context,
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

        // Destination Groups (słownik)
        await SeedDestinationGroupsAsync();

        // Tariffs & Rates (jeśli showcase włączony)
        if (seedConfig.GetValue<bool>("IncludeShowcaseData"))
        {
            await SeedTariffsAsync();
            await SeedRatesAsync();
        }

        _logger.LogInformation("Database seed completed");
    }

    private async Task SeedDestinationGroupsAsync()
    {
        var groups = ShowcaseData.GetDestinationGroups();

        foreach (var group in groups)
        {
            var exists = await _context.DestinationGroups
                .AnyAsync(g => g.Name == group.Name);

            if (!exists)
            {
                _context.DestinationGroups.Add(group);
                _logger.LogInformation("Seeded destination group: {Name}", group.Name);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedTariffsAsync()
    {
        var tariffs = ShowcaseData.GetTariffs();

        foreach (var tariff in tariffs)
        {
            var exists = await _context.Tariffs
                .AnyAsync(t => t.Gid == tariff.Gid);

            if (!exists)
            {
                _context.Tariffs.Add(tariff);
                _logger.LogInformation("Seeded tariff: {Name}", tariff.Name);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedRatesAsync()
    {
        // Pobierz ID taryf
        var standardTariff = await _context.Tariffs
            .FirstOrDefaultAsync(t => t.Gid == FixedGuids.TariffStandard);
        var premiumTariff = await _context.Tariffs
            .FirstOrDefaultAsync(t => t.Gid == FixedGuids.TariffPremium);

        if (standardTariff == null || premiumTariff == null)
        {
            _logger.LogWarning("Cannot seed rates - tariffs not found");
            return;
        }

        var rates = ShowcaseData.GetRates(standardTariff.Id, premiumTariff.Id);

        foreach (var rate in rates)
        {
            var exists = await _context.Rates
                .AnyAsync(r => r.Gid == rate.Gid);

            if (!exists)
            {
                _context.Rates.Add(rate);
                _logger.LogInformation("Seeded rate: {Prefix} - {Name}", rate.Prefix, rate.DestinationName);
            }
        }

        await _context.SaveChangesAsync();
    }
}
