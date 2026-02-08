namespace FinCosts.Api.Features.Costs.Model;

public class VatRateTypeResponse
{
    public int Id { get; set; }
    public string NamePL { get; set; } = "";
    public string NameEN { get; set; } = "";
    public decimal VatRate { get; set; }
}
