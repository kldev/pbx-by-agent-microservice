using CdrService.Data.Entities;

namespace CdrService.Api.Seed;

public static class ShowcaseData
{
    public static List<CallStatus> GetCallStatuses() =>
    [
        new() { Id = 1, Gid = FixedGuids.StatusCompleted, Code = "COMPLETED", NamePL = "Zakończone", NameEN = "Completed", Description = "Połączenie zakończone pomyślnie", SortOrder = 1 },
        new() { Id = 2, Gid = FixedGuids.StatusFailed, Code = "FAILED", NamePL = "Nieudane", NameEN = "Failed", Description = "Połączenie nie powiodło się", SortOrder = 2 },
        new() { Id = 3, Gid = FixedGuids.StatusBusy, Code = "BUSY", NamePL = "Zajęty", NameEN = "Busy", Description = "Numer zajęty", SortOrder = 3 },
        new() { Id = 4, Gid = FixedGuids.StatusNoAnswer, Code = "NO_ANSWER", NamePL = "Brak odpowiedzi", NameEN = "No Answer", Description = "Brak odpowiedzi", SortOrder = 4 },
        new() { Id = 5, Gid = FixedGuids.StatusCancelled, Code = "CANCELLED", NamePL = "Anulowane", NameEN = "Cancelled", Description = "Połączenie anulowane przez dzwoniącego", SortOrder = 5 },
        new() { Id = 6, Gid = FixedGuids.StatusRinging, Code = "RINGING", NamePL = "Dzwonienie", NameEN = "Ringing", Description = "Połączenie w trakcie dzwonienia (przerwane)", SortOrder = 6 }
    ];

    public static List<TerminationCause> GetTerminationCauses() =>
    [
        new() { Id = 1, Gid = FixedGuids.CauseNormalClearing, Code = "NORMAL_CLEARING", Q850Code = 16, NamePL = "Normalne rozłączenie", NameEN = "Normal Clearing", SortOrder = 1 },
        new() { Id = 2, Gid = FixedGuids.CauseUserBusy, Code = "USER_BUSY", Q850Code = 17, NamePL = "Użytkownik zajęty", NameEN = "User Busy", SortOrder = 2 },
        new() { Id = 3, Gid = FixedGuids.CauseNoAnswer, Code = "NO_ANSWER", Q850Code = 18, NamePL = "Brak odpowiedzi od użytkownika", NameEN = "No User Response", SortOrder = 3 },
        new() { Id = 4, Gid = FixedGuids.CauseCallRejected, Code = "CALL_REJECTED", Q850Code = 21, NamePL = "Połączenie odrzucone", NameEN = "Call Rejected", SortOrder = 4 },
        new() { Id = 5, Gid = FixedGuids.CauseNumberChanged, Code = "NUMBER_CHANGED", Q850Code = 22, NamePL = "Numer zmieniony", NameEN = "Number Changed", SortOrder = 5 },
        new() { Id = 6, Gid = FixedGuids.CauseDestinationOutOfOrder, Code = "DESTINATION_OUT_OF_ORDER", Q850Code = 27, NamePL = "Cel nieosiągalny", NameEN = "Destination Out of Order", SortOrder = 6 },
        new() { Id = 7, Gid = FixedGuids.CauseInvalidNumberFormat, Code = "INVALID_NUMBER_FORMAT", Q850Code = 28, NamePL = "Nieprawidłowy format numeru", NameEN = "Invalid Number Format", SortOrder = 7 },
        new() { Id = 8, Gid = FixedGuids.CauseNetworkOutOfOrder, Code = "NETWORK_OUT_OF_ORDER", Q850Code = 38, NamePL = "Sieć niedostępna", NameEN = "Network Out of Order", SortOrder = 8 },
        new() { Id = 9, Gid = FixedGuids.CauseTemporaryFailure, Code = "TEMPORARY_FAILURE", Q850Code = 41, NamePL = "Tymczasowa awaria", NameEN = "Temporary Failure", SortOrder = 9 },
        new() { Id = 10, Gid = FixedGuids.CauseCongestion, Code = "CONGESTION", Q850Code = 42, NamePL = "Przeciążenie sieci", NameEN = "Network Congestion", SortOrder = 10 }
    ];

    public static List<CallDirection> GetCallDirections() =>
    [
        new() { Id = 1, Gid = FixedGuids.DirectionInbound, Code = "INBOUND", NamePL = "Przychodzące", NameEN = "Inbound", Description = "Połączenie przychodzące z zewnątrz", SortOrder = 1 },
        new() { Id = 2, Gid = FixedGuids.DirectionOutbound, Code = "OUTBOUND", NamePL = "Wychodzące", NameEN = "Outbound", Description = "Połączenie wychodzące na zewnątrz", SortOrder = 2 },
        new() { Id = 3, Gid = FixedGuids.DirectionInternal, Code = "INTERNAL", NamePL = "Wewnętrzne", NameEN = "Internal", Description = "Połączenie wewnętrzne w systemie", SortOrder = 3 }
    ];

    public static List<CallRecord> GetSampleCallRecords() =>
    [
        new()
        {
            Gid = FixedGuids.CallRecordSample1,
            CallUuid = "sample-uuid-001",
            CallerId = "+48501234567",
            CalledNumber = "+48221234567",
            StartTime = DateTime.UtcNow.AddHours(-2),
            AnswerTime = DateTime.UtcNow.AddHours(-2).AddSeconds(5),
            EndTime = DateTime.UtcNow.AddHours(-2).AddMinutes(3).AddSeconds(25),
            Duration = 205,
            BillableSeconds = 200,
            CallStatusId = 1, // Completed
            TerminationCauseId = 1, // Normal Clearing
            CallDirectionId = 2, // Outbound
            TariffGid = "t1111111111111111111111111111111",
            TariffName = "Standard",
            RatePerMinute = 0.15m,
            ConnectionFee = 0,
            BillingIncrement = 60,
            CurrencyCode = "PLN",
            DestinationName = "Poland Mobile & Landline",
            MatchedPrefix = "+48",
            TotalCost = 0.60m,
            CustomerGid = "cust0001000000000000000000000001",
            CustomerName = "Przykładowy Klient Sp. z o.o.",
            SipAccountGid = "sip00001000000000000000000000001",
            SipAccountUsername = "user001@pbx.example.com"
        },
        new()
        {
            Gid = FixedGuids.CallRecordSample2,
            CallUuid = "sample-uuid-002",
            CallerId = "+48501234567",
            CalledNumber = "+49301234567",
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = DateTime.UtcNow.AddHours(-1).AddSeconds(30),
            Duration = 30,
            BillableSeconds = 0,
            CallStatusId = 4, // No Answer
            TerminationCauseId = 3, // No Answer
            CallDirectionId = 2, // Outbound
            TariffGid = "t1111111111111111111111111111111",
            TariffName = "Standard",
            RatePerMinute = 0.25m,
            ConnectionFee = 0,
            BillingIncrement = 60,
            CurrencyCode = "PLN",
            DestinationName = "Germany",
            MatchedPrefix = "+49",
            TotalCost = 0,
            CustomerGid = "cust0001000000000000000000000001",
            CustomerName = "Przykładowy Klient Sp. z o.o.",
            SipAccountGid = "sip00001000000000000000000000001",
            SipAccountUsername = "user001@pbx.example.com"
        }
    ];
}
