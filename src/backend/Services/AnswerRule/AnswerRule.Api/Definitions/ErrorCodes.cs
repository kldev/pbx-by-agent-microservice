using App.Shared.Web;

namespace AnswerRule.Api.Definitions;

public static class ErrorCodes
{
    public static class Rule
    {
        [ErrorMessage("Reguła nie została znaleziona")]
        public const string NotFound = "rule.not_found";

        [ErrorMessage("Nazwa reguły jest wymagana")]
        public const string NameRequired = "rule.name_required";

        [ErrorMessage("Konto SIP jest wymagane")]
        public const string SipAccountRequired = "rule.sip_account_required";

        [ErrorMessage("Cel akcji jest wymagany dla tego typu reguły")]
        public const string ActionTargetRequired = "rule.action_target_required";

        [ErrorMessage("Skrzynka voicemail jest wymagana dla tego typu reguły")]
        public const string VoicemailBoxRequired = "rule.voicemail_box_required";

        [ErrorMessage("Komunikat głosowy jest wymagany dla tego typu reguły")]
        public const string VoiceMessageRequired = "rule.voice_message_required";

        [ErrorMessage("Reguła musi mieć co najmniej jeden przedział czasowy")]
        public const string TimeSlotRequired = "rule.timeslot_required";

        [ErrorMessage("Nieprawidłowy format adresu email")]
        public const string InvalidEmailFormat = "rule.invalid_email_format";
    }

    public static class TimeSlot
    {
        [ErrorMessage("Przedział czasowy nie został znaleziony")]
        public const string NotFound = "timeslot.not_found";

        [ErrorMessage("Czas rozpoczęcia musi być wielokrotnością 15 minut")]
        public const string InvalidStartTimeGranularity = "timeslot.invalid_start_time_granularity";

        [ErrorMessage("Czas zakończenia musi być wielokrotnością 15 minut")]
        public const string InvalidEndTimeGranularity = "timeslot.invalid_end_time_granularity";

        [ErrorMessage("Czas zakończenia musi być większy niż czas rozpoczęcia")]
        public const string EndTimeBeforeStartTime = "timeslot.end_time_before_start_time";

        [ErrorMessage("Przedziały czasowe nie mogą się nakładać")]
        public const string OverlappingSlots = "timeslot.overlapping_slots";

        [ErrorMessage("Nieprawidłowy dzień tygodnia")]
        public const string InvalidDayOfWeek = "timeslot.invalid_day_of_week";
    }
}
