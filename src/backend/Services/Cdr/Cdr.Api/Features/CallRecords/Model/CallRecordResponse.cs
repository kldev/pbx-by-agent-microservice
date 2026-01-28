namespace CdrService.Api.Features.CallRecords.Model;

public class CallRecordResponse
{
    public string Gid { get; set; } = null!;
    public string CallerId { get; set; } = null!;
    public string CalledNumber { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Duration { get; set; }
    public int BillableSeconds { get; set; }
    public string CallStatusCode { get; set; } = null!;
    public string CallStatusName { get; set; } = null!;
    public string CallDirectionCode { get; set; } = null!;
    public decimal TotalCost { get; set; }
    public string CurrencyCode { get; set; } = null!;
    public string? CustomerName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CallRecordDetailResponse : CallRecordResponse
{
    public string? CallUuid { get; set; }
    public DateTime? AnswerTime { get; set; }
    public string? TerminationCauseCode { get; set; }
    public string? TerminationCauseName { get; set; }
    public string? SourceGatewayGid { get; set; }
    public string? SourceGatewayName { get; set; }
    public string? DestinationGatewayGid { get; set; }
    public string? DestinationGatewayName { get; set; }
    public string? TariffGid { get; set; }
    public string? TariffName { get; set; }
    public decimal RatePerMinute { get; set; }
    public decimal ConnectionFee { get; set; }
    public int BillingIncrement { get; set; }
    public string? DestinationName { get; set; }
    public string? MatchedPrefix { get; set; }
    public string? CustomerGid { get; set; }
    public string? SipAccountGid { get; set; }
    public string? SipAccountUsername { get; set; }
    public string? UserData { get; set; }
}
