using App.Shared.Web;

namespace CdrService.Api.Definitions;

public static class ErrorCodes
{
    public static class CallRecord
    {
        [ErrorMessage("Rekord CDR nie został znaleziony")]
        public const string NotFound = "call_record.not_found";

        [ErrorMessage("Numer dzwoniącego jest wymagany")]
        public const string CallerIdRequired = "call_record.caller_id_required";

        [ErrorMessage("Numer wywoływany jest wymagany")]
        public const string CalledNumberRequired = "call_record.called_number_required";

        [ErrorMessage("Nieprawidłowy zakres dat")]
        public const string InvalidDateRange = "call_record.invalid_date_range";

        [ErrorMessage("Data zakończenia musi być późniejsza niż data rozpoczęcia")]
        public const string EndTimeBeforeStartTime = "call_record.end_time_before_start_time";

        [ErrorMessage("Status połączenia jest wymagany")]
        public const string CallStatusRequired = "call_record.call_status_required";

        [ErrorMessage("Przyczyna zakończenia jest wymagana")]
        public const string TerminationCauseRequired = "call_record.termination_cause_required";

        [ErrorMessage("Kierunek połączenia jest wymagany")]
        public const string CallDirectionRequired = "call_record.call_direction_required";

        [ErrorMessage("Nieprawidłowy status połączenia")]
        public const string InvalidCallStatus = "call_record.invalid_call_status";

        [ErrorMessage("Nieprawidłowa przyczyna zakończenia")]
        public const string InvalidTerminationCause = "call_record.invalid_termination_cause";

        [ErrorMessage("Nieprawidłowy kierunek połączenia")]
        public const string InvalidCallDirection = "call_record.invalid_call_direction";

        [ErrorMessage("Czas trwania nie może być ujemny")]
        public const string InvalidDuration = "call_record.invalid_duration";

        [ErrorMessage("Koszt nie może być ujemny")]
        public const string InvalidCost = "call_record.invalid_cost";
    }

    public static class CallStatus
    {
        [ErrorMessage("Status połączenia nie został znaleziony")]
        public const string NotFound = "call_status.not_found";
    }

    public static class TerminationCause
    {
        [ErrorMessage("Przyczyna zakończenia nie została znaleziona")]
        public const string NotFound = "termination_cause.not_found";
    }

    public static class CallDirection
    {
        [ErrorMessage("Kierunek połączenia nie został znaleziony")]
        public const string NotFound = "call_direction.not_found";
    }
}
