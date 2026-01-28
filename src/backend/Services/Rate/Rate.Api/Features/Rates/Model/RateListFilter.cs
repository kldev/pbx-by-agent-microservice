namespace RateService.Api.Features.Rates.Model;

public class RateListFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Search { get; set; }
    public string? TariffGid { get; set; }
    public string? Prefix { get; set; }
    public int? DestinationGroupId { get; set; }
    public bool? IsActive { get; set; }
}
