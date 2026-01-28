namespace RateService.Api.Features.Rates.Model;

public class CreateRateRequest
{
    public string TariffGid { get; set; } = null!;
    public string Prefix { get; set; } = null!;
    public string DestinationName { get; set; } = null!;
    public decimal RatePerMinute { get; set; }
    public decimal? ConnectionFee { get; set; }
    public int? BillingIncrement { get; set; }
    public int? MinimumDuration { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public int? DestinationGroupId { get; set; }
    public string? Notes { get; set; }
}
