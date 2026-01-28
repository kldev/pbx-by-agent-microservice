namespace RateService.Api.Features.Tariffs.Model;

public class CreateTariffRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CurrencyCode { get; set; } = "PLN";
    public bool IsDefault { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public int BillingIncrement { get; set; } = 60;
    public int MinimumDuration { get; set; } = 0;
    public decimal ConnectionFee { get; set; } = 0;
}
