using Microsoft.EntityFrameworkCore;
using CdrService.Api.Seed;
using CdrService.Data;
using CdrService.Data.Entities;

namespace CdrService.Api.Tests.Infrastructure;

public static class CdrTestDataSeeder
{
    public static readonly string TestCallRecordGid = "callrec-test-1111-1111-111111111111";
    public static readonly string TestCallStatusGid = FixedGuids.StatusCompleted;
    public static readonly string TestTerminationCauseGid = FixedGuids.CauseNormalClearing;
    public static readonly string TestCallDirectionGid = FixedGuids.DirectionOutbound;

    public static async Task SeedAsync(CdrDbContext context)
    {
        // Seed dictionaries if not exists
        if (!await context.CallStatuses.AnyAsync())
        {
            context.CallStatuses.AddRange(ShowcaseData.GetCallStatuses());
            await context.SaveChangesAsync();
        }

        if (!await context.TerminationCauses.AnyAsync())
        {
            context.TerminationCauses.AddRange(ShowcaseData.GetTerminationCauses());
            await context.SaveChangesAsync();
        }

        if (!await context.CallDirections.AnyAsync())
        {
            context.CallDirections.AddRange(ShowcaseData.GetCallDirections());
            await context.SaveChangesAsync();
        }

        // Seed test call record if not exists
        var existingRecord = await context.CallRecords
            .FirstOrDefaultAsync(c => c.Gid == TestCallRecordGid);

        if (existingRecord == null)
        {
            var testRecord = new CallRecord
            {
                Gid = TestCallRecordGid,
                CallUuid = "test-uuid-001",
                CallerId = "+48501111111",
                CalledNumber = "+48502222222",
                StartTime = DateTime.UtcNow.AddHours(-1),
                AnswerTime = DateTime.UtcNow.AddHours(-1).AddSeconds(5),
                EndTime = DateTime.UtcNow.AddHours(-1).AddMinutes(2),
                Duration = 125,
                BillableSeconds = 120,
                CallStatusId = 1, // Completed
                TerminationCauseId = 1, // Normal Clearing
                CallDirectionId = 2, // Outbound
                TariffGid = "test-tariff-gid",
                TariffName = "Test Tariff",
                RatePerMinute = 0.10m,
                ConnectionFee = 0,
                BillingIncrement = 60,
                CurrencyCode = "PLN",
                DestinationName = "Poland Test",
                MatchedPrefix = "+48",
                TotalCost = 0.20m,
                CustomerGid = "test-customer-gid",
                CustomerName = "Test Customer",
                SipAccountGid = "test-sip-gid",
                SipAccountUsername = "test@sip.local"
            };

            context.CallRecords.Add(testRecord);
            await context.SaveChangesAsync();
        }
    }
}
