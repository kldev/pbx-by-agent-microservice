using FinCosts.Data;
using FinCosts.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinCosts.Api.Seed;

public static class DocumentEntrySeedShowcase
{
    // 10 fixed GIDs for stable test/demo references
    private static readonly string[] FixedGids =
    [
        "a1b2c3d4-0001-4000-8000-000000000001",
        "a1b2c3d4-0002-4000-8000-000000000002",
        "a1b2c3d4-0003-4000-8000-000000000003",
        "a1b2c3d4-0004-4000-8000-000000000004",
        "a1b2c3d4-0005-4000-8000-000000000005",
        "a1b2c3d4-0006-4000-8000-000000000006",
        "a1b2c3d4-0007-4000-8000-000000000007",
        "a1b2c3d4-0008-4000-8000-000000000008",
        "a1b2c3d4-0009-4000-8000-000000000009",
        "a1b2c3d4-0010-4000-8000-000000000010"
    ];

    private static readonly string[] Suppliers =
    [
        "TechNova Sp. z o.o.",
        "BudMax S.A.",
        "DataSoft Solutions Sp. z o.o.",
        "GreenEnergy Polska S.A.",
        "MetalPro Sp. z o.o.",
        "CloudPoint Sp. z o.o.",
        "AutoParts Express S.A.",
        "FreshFood Logistics Sp. z o.o.",
        "SteelWorks Krakow S.A.",
        "PrintHouse Media Sp. z o.o.",
        "NetBridge Consulting S.A.",
        "AquaPure Systems Sp. z o.o.",
        "EuroTrans Sp. z o.o.",
        "SmartOffice Sp. z o.o.",
        "BioLab Diagnostics S.A."
    ];

    private static readonly string[] Buyers =
    [
        "Apex Solutions Sp. z o.o.",
        "Nova Industries S.A.",
        "Skyline Development Sp. z o.o.",
        "Horizon Trading S.A.",
        "BluePeak Sp. z o.o.",
        "Vertex Consulting Sp. z o.o.",
        "Atlas Logistics S.A.",
        "CrystalClear Software Sp. z o.o.",
        "PrimePath Finance S.A.",
        "RedOak Enterprises Sp. z o.o."
    ];

    private static readonly (string NamePL, decimal? Rate)[] Currencies =
    [
        ("PLN", null),
        ("EUR", 4.32m),
        ("USD", 3.98m),
        ("DKK", 0.58m)
    ];

    private static readonly (int Id, string NamePL)[] DocumentTypes =
    [
        (1, "Faktura"),
        (2, "Pro forma"),
        (99, "Inne")
    ];

    private static readonly decimal[] VatRates = [0m, 7m, 9m, 13m, 21m, 23m];

    public static async Task SeedDocuments(FinCostsDbContext context)
    {
        var hasAny = await context.DocumentEntryReadOnly.AnyAsync();
        if (hasAny) return;

        var entries = new List<DocumentEntry>();
        var rng = new Random(42); // deterministic seed for reproducibility

        // -- 10 fixed GID records --
        entries.AddRange(CreateFixedEntries());

        // -- random 100-500 records --
        var randomCount = rng.Next(100, 501);
        for (var i = 0; i < randomCount; i++)
        {
            entries.Add(CreateRandomEntry(rng));
        }

        await context.DocumentEntrySeed.AddRangeAsync(entries);
        await context.SaveChangesAsync();
    }

    private static List<DocumentEntry> CreateFixedEntries()
    {
        var baseDate = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        return
        [
            new()
            {
                Gid = FixedGids[0],
                CurrencyNamePL = "PLN",
                CurrencyForeignRate = null,
                TotalAmount = 12_350.00m,
                WasPaid = true,
                KSERefNumber = "KSE/2025/00001",
                KSEAccountNumber = "62102040270000110205929781",
                DocumentTypeId = 1,
                DocumentTypeNamePL = "Faktura",
                IssuedBy = "TechNova Sp. z o.o.",
                IssuedVatNumber = "PL5261234567",
                IssuedFor = "Apex Solutions Sp. z o.o.",
                IssuedForVatNumber = "PL7891234560",
                VatRate = 23m,
                CreatedAt = baseDate
            },
            new()
            {
                Gid = FixedGids[1],
                CurrencyNamePL = "EUR",
                CurrencyForeignRate = 4.32m,
                TotalAmount = 8_500.00m,
                WasPaid = true,
                KSERefNumber = "KSE/2025/00002",
                KSEAccountNumber = "27114020040000360278637692",
                DocumentTypeId = 1,
                DocumentTypeNamePL = "Faktura",
                IssuedBy = "BudMax S.A.",
                IssuedVatNumber = "PL6432198765",
                IssuedFor = "Nova Industries S.A.",
                IssuedForVatNumber = "PL1239876540",
                VatRate = 23m,
                CreatedAt = baseDate.AddDays(3)
            },
            new()
            {
                Gid = FixedGids[2],
                CurrencyNamePL = "PLN",
                CurrencyForeignRate = null,
                TotalAmount = 950.50m,
                WasPaid = false,
                KSERefNumber = null,
                KSEAccountNumber = null,
                DocumentTypeId = 2,
                DocumentTypeNamePL = "Pro forma",
                IssuedBy = "DataSoft Solutions Sp. z o.o.",
                IssuedVatNumber = "PL9871234560",
                IssuedFor = "Skyline Development Sp. z o.o.",
                IssuedForVatNumber = "PL4561237890",
                VatRate = 7m,
                CreatedAt = baseDate.AddDays(7)
            },
            new()
            {
                Gid = FixedGids[3],
                CurrencyNamePL = "USD",
                CurrencyForeignRate = 3.98m,
                TotalAmount = 25_000.00m,
                WasPaid = true,
                KSERefNumber = "KSE/2025/00004",
                KSEAccountNumber = "51109010140000071219812874",
                DocumentTypeId = 1,
                DocumentTypeNamePL = "Faktura",
                IssuedBy = "GreenEnergy Polska S.A.",
                IssuedVatNumber = "PL1112223334",
                IssuedFor = "Horizon Trading S.A.",
                IssuedForVatNumber = "PL5556667778",
                VatRate = 23m,
                CreatedAt = baseDate.AddDays(10)
            },
            new()
            {
                Gid = FixedGids[4],
                CurrencyNamePL = "PLN",
                CurrencyForeignRate = null,
                TotalAmount = 3_200.75m,
                WasPaid = false,
                KSERefNumber = null,
                KSEAccountNumber = null,
                DocumentTypeId = 99,
                DocumentTypeNamePL = "Inne",
                IssuedBy = "MetalPro Sp. z o.o.",
                IssuedVatNumber = "PL3334445556",
                IssuedFor = "BluePeak Sp. z o.o.",
                IssuedForVatNumber = "PL7778889990",
                VatRate = 0m,
                CreatedAt = baseDate.AddDays(14)
            },
            new()
            {
                Gid = FixedGids[5],
                CurrencyNamePL = "DKK",
                CurrencyForeignRate = 0.58m,
                TotalAmount = 41_200.00m,
                WasPaid = true,
                KSERefNumber = "KSE/2025/00006",
                KSEAccountNumber = "68105014611000009876543210",
                DocumentTypeId = 1,
                DocumentTypeNamePL = "Faktura",
                IssuedBy = "CloudPoint Sp. z o.o.",
                IssuedVatNumber = "PL2223334445",
                IssuedFor = "Vertex Consulting Sp. z o.o.",
                IssuedForVatNumber = "PL8889990001",
                VatRate = 21m,
                CreatedAt = baseDate.AddDays(18)
            },
            new()
            {
                Gid = FixedGids[6],
                CurrencyNamePL = "EUR",
                CurrencyForeignRate = 4.32m,
                TotalAmount = 1_750.00m,
                WasPaid = false,
                KSERefNumber = null,
                KSEAccountNumber = null,
                DocumentTypeId = 2,
                DocumentTypeNamePL = "Pro forma",
                IssuedBy = "AutoParts Express S.A.",
                IssuedVatNumber = "PL4445556667",
                IssuedFor = "Atlas Logistics S.A.",
                IssuedForVatNumber = "PL6667778889",
                VatRate = 9m,
                CreatedAt = baseDate.AddDays(21)
            },
            new()
            {
                Gid = FixedGids[7],
                CurrencyNamePL = "PLN",
                CurrencyForeignRate = null,
                TotalAmount = 67_800.00m,
                WasPaid = true,
                KSERefNumber = "KSE/2025/00008",
                KSEAccountNumber = "95116022020000000348795012",
                DocumentTypeId = 1,
                DocumentTypeNamePL = "Faktura",
                IssuedBy = "FreshFood Logistics Sp. z o.o.",
                IssuedVatNumber = "PL5556667778",
                IssuedFor = "CrystalClear Software Sp. z o.o.",
                IssuedForVatNumber = "PL9990001112",
                VatRate = 23m,
                CreatedAt = baseDate.AddDays(25)
            },
            new()
            {
                Gid = FixedGids[8],
                CurrencyNamePL = "PLN",
                CurrencyForeignRate = null,
                TotalAmount = 4_999.99m,
                WasPaid = false,
                KSERefNumber = null,
                KSEAccountNumber = null,
                DocumentTypeId = 99,
                DocumentTypeNamePL = "Inne",
                IssuedBy = "SteelWorks Krakow S.A.",
                IssuedVatNumber = "PL6667778889",
                IssuedFor = "PrimePath Finance S.A.",
                IssuedForVatNumber = "PL1110009998",
                VatRate = 13m,
                CreatedAt = baseDate.AddDays(28)
            },
            new()
            {
                Gid = FixedGids[9],
                CurrencyNamePL = "USD",
                CurrencyForeignRate = 3.98m,
                TotalAmount = 15_600.00m,
                WasPaid = true,
                KSERefNumber = "KSE/2025/00010",
                KSEAccountNumber = "82103015080000000500207091",
                DocumentTypeId = 1,
                DocumentTypeNamePL = "Faktura",
                IssuedBy = "PrintHouse Media Sp. z o.o.",
                IssuedVatNumber = "PL7778889990",
                IssuedFor = "RedOak Enterprises Sp. z o.o.",
                IssuedForVatNumber = "PL2221110009",
                VatRate = 23m,
                CreatedAt = baseDate.AddDays(30)
            }
        ];
    }

    private static DocumentEntry CreateRandomEntry(Random rng)
    {
        var currency = Currencies[rng.Next(Currencies.Length)];
        var docType = DocumentTypes[rng.Next(DocumentTypes.Length)];
        var vatRate = VatRates[rng.Next(VatRates.Length)];
        var supplier = Suppliers[rng.Next(Suppliers.Length)];
        var buyer = Buyers[rng.Next(Buyers.Length)];
        var wasPaid = rng.Next(100) < 65; // 65% paid
        var daysAgo = rng.Next(1, 365);
        var hasKse = wasPaid && rng.Next(100) < 70; // 70% of paid have KSE

        return new DocumentEntry
        {
            Gid = Guid.NewGuid().ToString(),
            CurrencyNamePL = currency.NamePL,
            CurrencyForeignRate = currency.Rate,
            TotalAmount = Math.Round((decimal)(rng.NextDouble() * 99_900 + 100), 2), // 100 - 100_000
            WasPaid = wasPaid,
            KSERefNumber = hasKse ? $"KSE/{2025 - daysAgo / 365}/{rng.Next(10000, 99999):D5}" : null,
            KSEAccountNumber = hasKse ? GenerateFakeAccountNumber(rng) : null,
            DocumentTypeId = docType.Id,
            DocumentTypeNamePL = docType.NamePL,
            VatRate = vatRate,
            IssuedBy = supplier,
            IssuedVatNumber = $"PL{rng.Next(1000000000, int.MaxValue)}",
            IssuedFor = buyer,
            IssuedForVatNumber = $"PL{rng.Next(1000000000, int.MaxValue)}",
            CreatedAt = DateTime.UtcNow.AddDays(-daysAgo).AddHours(rng.Next(0, 24)).AddMinutes(rng.Next(0, 60))
        };
    }

    private static string GenerateFakeAccountNumber(Random rng)
    {
        var sb = new System.Text.StringBuilder(26);
        for (var i = 0; i < 26; i++)
            sb.Append(rng.Next(0, 10));
        return sb.ToString();
    }
}
