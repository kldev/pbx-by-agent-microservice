using App.BaseData;

namespace CdrService.Data.Entities;

/// <summary>
/// Call Data Record - rekord pojedynczego połączenia telefonicznego
/// </summary>
public class CallRecord : BaseAuditableTable
{
    // === Identyfikacja połączenia ===

    /// <summary>
    /// Unikalny identyfikator połączenia z systemu telefonicznego
    /// </summary>
    public string? CallUuid { get; set; }

    /// <summary>
    /// Numer dzwoniącego (format E.164)
    /// </summary>
    public string CallerId { get; set; } = null!;

    /// <summary>
    /// Numer wywoływany (format E.164)
    /// </summary>
    public string CalledNumber { get; set; } = null!;

    // === Informacje czasowe ===

    /// <summary>
    /// Czas rozpoczęcia połączenia (inicjacja)
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Czas odebrania połączenia (null jeśli nieodebrane)
    /// </summary>
    public DateTime? AnswerTime { get; set; }

    /// <summary>
    /// Czas zakończenia połączenia
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Całkowity czas trwania w sekundach (włącznie z dzwonieniem)
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Czas do rozliczenia w sekundach (tylko rozmowa)
    /// </summary>
    public int BillableSeconds { get; set; }

    // === Status i wynik ===

    /// <summary>
    /// Status połączenia (FK do słownika CallStatus)
    /// </summary>
    public long CallStatusId { get; set; }
    public CallStatus? CallStatus { get; set; }

    /// <summary>
    /// Przyczyna zakończenia (FK do słownika TerminationCause)
    /// </summary>
    public long TerminationCauseId { get; set; }
    public TerminationCause? TerminationCause { get; set; }

    /// <summary>
    /// Kierunek połączenia (FK do słownika CallDirection)
    /// </summary>
    public long CallDirectionId { get; set; }
    public CallDirection? CallDirection { get; set; }

    // === Informacje o bramach ===

    /// <summary>
    /// GID bramy źródłowej
    /// </summary>
    public string? SourceGatewayGid { get; set; }

    /// <summary>
    /// Nazwa bramy źródłowej (snapshot)
    /// </summary>
    public string? SourceGatewayName { get; set; }

    /// <summary>
    /// GID bramy docelowej
    /// </summary>
    public string? DestinationGatewayGid { get; set; }

    /// <summary>
    /// Nazwa bramy docelowej (snapshot)
    /// </summary>
    public string? DestinationGatewayName { get; set; }

    // === SNAPSHOT: Informacje o taryfie/stawce ===

    /// <summary>
    /// GID taryfy w momencie połączenia
    /// </summary>
    public string? TariffGid { get; set; }

    /// <summary>
    /// Nazwa taryfy (snapshot z Rate service)
    /// </summary>
    public string? TariffName { get; set; }

    /// <summary>
    /// Stawka za minutę (snapshot z Rate service)
    /// </summary>
    public decimal RatePerMinute { get; set; }

    /// <summary>
    /// Opłata za połączenie (snapshot z Rate service)
    /// </summary>
    public decimal ConnectionFee { get; set; }

    /// <summary>
    /// Interwał naliczania w sekundach (snapshot z Rate service)
    /// </summary>
    public int BillingIncrement { get; set; } = 60;

    /// <summary>
    /// Kod waluty (snapshot z Rate service)
    /// </summary>
    public string CurrencyCode { get; set; } = "PLN";

    /// <summary>
    /// Nazwa kierunku ze stawki (snapshot)
    /// </summary>
    public string? DestinationName { get; set; }

    /// <summary>
    /// Dopasowany prefiks ze stawki (snapshot)
    /// </summary>
    public string? MatchedPrefix { get; set; }

    // === Rozliczenie ===

    /// <summary>
    /// Całkowity koszt połączenia
    /// </summary>
    public decimal TotalCost { get; set; }

    // === SNAPSHOT: Informacje o kliencie ===

    /// <summary>
    /// GID klienta w momencie połączenia
    /// </summary>
    public string? CustomerGid { get; set; }

    /// <summary>
    /// Nazwa klienta (snapshot)
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// GID konta SIP wykonującego/odbierającego połączenie
    /// </summary>
    public string? SipAccountGid { get; set; }

    /// <summary>
    /// Username konta SIP (snapshot)
    /// </summary>
    public string? SipAccountUsername { get; set; }

    // === Dodatkowe informacje ===

    /// <summary>
    /// Dane użytkownika lub notatki
    /// </summary>
    public string? UserData { get; set; }

    /// <summary>
    /// Surowe dane CDR z systemu telefonicznego (JSON)
    /// </summary>
    public string? RawCdrJson { get; set; }
}
