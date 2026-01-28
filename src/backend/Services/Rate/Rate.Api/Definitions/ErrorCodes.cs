using App.Shared.Web;

namespace RateService.Api.Definitions;

public static class ErrorCodes
{
    public static class Tariff
    {
        [ErrorMessage("Taryfa nie została znaleziona")]
        public const string NotFound = "tariff.not_found";

        [ErrorMessage("Nazwa taryfy jest wymagana")]
        public const string NameRequired = "tariff.name_required";

        [ErrorMessage("Taryfa o tej nazwie już istnieje")]
        public const string NameExists = "tariff.name_exists";

        [ErrorMessage("Interwał naliczania musi być większy od zera")]
        public const string InvalidBillingIncrement = "tariff.invalid_billing_increment";

        [ErrorMessage("Nie można usunąć domyślnej taryfy")]
        public const string CannotDeleteDefault = "tariff.cannot_delete_default";

        [ErrorMessage("Waluta jest wymagana")]
        public const string CurrencyRequired = "tariff.currency_required";
    }

    public static class Rate
    {
        [ErrorMessage("Stawka nie została znaleziona")]
        public const string NotFound = "rate.not_found";

        [ErrorMessage("Prefiks jest wymagany")]
        public const string PrefixRequired = "rate.prefix_required";

        [ErrorMessage("Nazwa destynacji jest wymagana")]
        public const string DestinationNameRequired = "rate.destination_name_required";

        [ErrorMessage("Stawka za minutę nie może być ujemna")]
        public const string InvalidRatePerMinute = "rate.invalid_rate_per_minute";

        [ErrorMessage("Prefiks już istnieje w tej taryfie")]
        public const string PrefixExists = "rate.prefix_exists";

        [ErrorMessage("Brak stawki dla podanego numeru")]
        public const string NoRateForNumber = "rate.no_rate_for_number";

        [ErrorMessage("Taryfa nie istnieje")]
        public const string TariffNotFound = "rate.tariff_not_found";
    }

    public static class DestinationGroup
    {
        [ErrorMessage("Grupa destynacji nie została znaleziona")]
        public const string NotFound = "destination_group.not_found";

        [ErrorMessage("Nazwa grupy jest wymagana")]
        public const string NameRequired = "destination_group.name_required";

        [ErrorMessage("Grupa o tej nazwie już istnieje")]
        public const string NameExists = "destination_group.name_exists";
    }
}
