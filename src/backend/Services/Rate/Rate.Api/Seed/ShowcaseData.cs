using RateService.Data.Entities;

namespace RateService.Api.Seed;

public static class ShowcaseData
{
    public static List<DestinationGroup> GetDestinationGroups() => new()
    {
        new DestinationGroup { Id = 1, Name = "Europe", NamePL = "Europa", NameEN = "Europe", Description = "Kraje europejskie" },
        new DestinationGroup { Id = 2, Name = "NorthAmerica", NamePL = "Ameryka Północna", NameEN = "North America", Description = "USA, Kanada" },
        new DestinationGroup { Id = 3, Name = "Asia", NamePL = "Azja", NameEN = "Asia", Description = "Kraje azjatyckie" },
        new DestinationGroup { Id = 4, Name = "Premium", NamePL = "Premium", NameEN = "Premium", Description = "Numery premium" },
        new DestinationGroup { Id = 5, Name = "Mobile", NamePL = "Komórkowe", NameEN = "Mobile", Description = "Numery komórkowe" }
    };

    public static List<Tariff> GetTariffs() => new()
    {
        new Tariff
        {
            Gid = FixedGuids.TariffStandard,
            Name = "Standard",
            Description = "Standardowa taryfa dla klientów",
            CurrencyCode = "PLN",
            IsDefault = true,
            BillingIncrement = 60,
            MinimumDuration = 0,
            ConnectionFee = 0
        },
        new Tariff
        {
            Gid = FixedGuids.TariffPremium,
            Name = "Premium",
            Description = "Taryfa dla klientów Premium z niższymi stawkami",
            CurrencyCode = "PLN",
            IsDefault = false,
            BillingIncrement = 1,
            MinimumDuration = 0,
            ConnectionFee = 0
        }
    };

    public static List<Rate> GetRates(long standardTariffId, long premiumTariffId) => new()
    {
        // Poland - Standard
        new Rate
        {
            Gid = FixedGuids.RatePolandMobile,
            TariffId = standardTariffId,
            Prefix = "+48",
            DestinationName = "Poland Mobile & Landline",
            RatePerMinute = 0.15m,
            DestinationGroupId = 1
        },

        // Germany - Standard
        new Rate
        {
            Gid = FixedGuids.RateGermanyMobile,
            TariffId = standardTariffId,
            Prefix = "+49",
            DestinationName = "Germany",
            RatePerMinute = 0.25m,
            DestinationGroupId = 1
        },

        // USA - Standard
        new Rate
        {
            Gid = FixedGuids.RateUSAGeneral,
            TariffId = standardTariffId,
            Prefix = "+1",
            DestinationName = "USA & Canada",
            RatePerMinute = 0.10m,
            DestinationGroupId = 2
        },

        // Premium rates (lower prices)
        new Rate
        {
            Gid = FixedGuids.RatePolandLandline,
            TariffId = premiumTariffId,
            Prefix = "+48",
            DestinationName = "Poland Mobile & Landline",
            RatePerMinute = 0.08m,
            DestinationGroupId = 1
        },
        new Rate
        {
            Gid = FixedGuids.RateGermanyLandline,
            TariffId = premiumTariffId,
            Prefix = "+49",
            DestinationName = "Germany",
            RatePerMinute = 0.15m,
            DestinationGroupId = 1
        }
    };
}
