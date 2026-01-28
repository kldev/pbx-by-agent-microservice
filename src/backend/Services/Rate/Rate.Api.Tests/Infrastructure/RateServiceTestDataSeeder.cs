using Microsoft.EntityFrameworkCore;
using RateService.Data;
using RateService.Data.Entities;

namespace RateService.Api.Tests.Infrastructure;

public static class RateServiceTestDataSeeder
{
    public static readonly string TestTariffGid = "tariff-test-1111-1111-111111111111";
    public static readonly string TestRateGid = "rate-test-1111-1111-111111111111";
    public const string TestGroupName = "TestGroup";

    public static async Task SeedAsync(RateServiceDbContext context)
    {
        // Destination Group - sprawdź czy już istnieje
        var group = await context.DestinationGroups
            .FirstOrDefaultAsync(g => g.Name == TestGroupName);

        if (group == null)
        {
            group = new DestinationGroup
            {
                Name = TestGroupName,
                NamePL = "Grupa Testowa",
                NameEN = "Test Group"
            };
            context.DestinationGroups.Add(group);
            await context.SaveChangesAsync();
        }

        // Taryfa testowa - sprawdź czy już istnieje
        var tariff = await context.Tariffs
            .FirstOrDefaultAsync(t => t.Gid == TestTariffGid);

        if (tariff == null)
        {
            tariff = new Tariff
            {
                Gid = TestTariffGid,
                Name = "Test Tariff",
                Description = "Taryfa do testów",
                CurrencyCode = "PLN",
                IsDefault = true,
                BillingIncrement = 60,
                MinimumDuration = 0,
                ConnectionFee = 0
            };
            context.Tariffs.Add(tariff);
            await context.SaveChangesAsync();
        }

        // Stawka testowa - sprawdź czy już istnieje
        var rate = await context.Rates
            .FirstOrDefaultAsync(r => r.Gid == TestRateGid);

        if (rate == null)
        {
            rate = new Rate
            {
                Gid = TestRateGid,
                TariffId = tariff.Id,
                Prefix = "+48",
                DestinationName = "Poland Test",
                RatePerMinute = 0.10m,
                DestinationGroupId = group.Id
            };
            context.Rates.Add(rate);
            await context.SaveChangesAsync();
        }
    }
}
