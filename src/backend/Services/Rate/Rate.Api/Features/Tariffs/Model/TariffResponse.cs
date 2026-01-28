namespace RateService.Api.Features.Tariffs.Model;

public class TariffResponse
{
    public string Gid { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CurrencyCode { get; set; } = null!;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public int BillingIncrement { get; set; }
    public int MinimumDuration { get; set; }
    public decimal ConnectionFee { get; set; }
    public int RatesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TariffDetailResponse : TariffResponse
{
    public IEnumerable<TariffRateResponse> Rates { get; set; } = new List<TariffRateResponse>();
}

public class TariffRateResponse
{
    public string Gid { get; set; } = null!;
    public string Prefix { get; set; } = null!;
    public string DestinationName { get; set; } = null!;
    public decimal RatePerMinute { get; set; }
    public bool IsActive { get; set; }
}
