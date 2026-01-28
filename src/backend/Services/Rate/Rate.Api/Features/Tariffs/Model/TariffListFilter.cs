namespace RateService.Api.Features.Tariffs.Model;

public class TariffListFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public string? CurrencyCode { get; set; }
}
