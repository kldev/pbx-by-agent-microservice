namespace CdrService.Api.Features.CallRecords.Model;

public class CreateCallRecordRequest
{
    // Identyfikacja
    public string? CallUuid { get; set; }
    public string CallerId { get; set; } = null!;
    public string CalledNumber { get; set; } = null!;

    // Czas
    public DateTime StartTime { get; set; }
    public DateTime? AnswerTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Duration { get; set; }
    public int BillableSeconds { get; set; }

    // Status
    public int CallStatusId { get; set; }
    public int TerminationCauseId { get; set; }
    public int CallDirectionId { get; set; }

    // Gateway
    public string? SourceGatewayGid { get; set; }
    public string? SourceGatewayName { get; set; }
    public string? DestinationGatewayGid { get; set; }
    public string? DestinationGatewayName { get; set; }

    // Snapshot: Taryfa
    public string? TariffGid { get; set; }
    public string? TariffName { get; set; }
    public decimal RatePerMinute { get; set; }
    public decimal ConnectionFee { get; set; }
    public int BillingIncrement { get; set; } = 60;
    public string CurrencyCode { get; set; } = "PLN";
    public string? DestinationName { get; set; }
    public string? MatchedPrefix { get; set; }

    // Rozliczenie
    public decimal TotalCost { get; set; }

    // Snapshot: Klient
    public string? CustomerGid { get; set; }
    public string? CustomerName { get; set; }
    public string? SipAccountGid { get; set; }
    public string? SipAccountUsername { get; set; }

    // Dodatkowe
    public string? UserData { get; set; }
    public string? RawCdrJson { get; set; }
}
