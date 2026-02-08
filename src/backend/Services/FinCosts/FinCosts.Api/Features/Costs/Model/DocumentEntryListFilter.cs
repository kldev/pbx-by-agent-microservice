using App.Shared.Web.BaseModel;

namespace FinCosts.Api.Features.Costs.Model;

public class DocumentEntryListFilter : PagedRequest
{
    public string? Search { get; set; }
    public int? DocumentTypeId { get; set; }
    public string? CurrencyNamePL { get; set; }
    public bool? WasPaid { get; set; }
}
