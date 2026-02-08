namespace FinCosts.Api.Features.Costs.Model;

public class DocumentEntryResponse
{
    public string Gid { get; set; } = "";
    public string CurrencyNamePL { get; set; } = "";
    public decimal? CurrencyForeignRate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal VatRate { get; set; }
    public bool WasPaid { get; set; }
    public string? KSERefNumber { get; set; }
    public string? KSEAccountNumber { get; set; }
    public int DocumentTypeId { get; set; }
    public string DocumentTypeNamePL { get; set; } = "";
    public string IssuedBy { get; set; } = "";
    public string? IssuedVatNumber { get; set; }
    public string IssuedFor { get; set; } = "";
    public string IssuedForVatNumber { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
