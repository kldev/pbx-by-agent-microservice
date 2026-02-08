using App.BaseData;

namespace FinCosts.Data.Entities;

public class DocumentEntry: BaseAuditableTable
{
    public string CurrencyNamePL { get; set; } = "";
    public decimal? CurrencyForeignRate { get; set; } = null;
    public decimal TotalAmount { get; set; } = 0;
    public bool WasPaid { get; set; } = false;
    public string? KSERefNumber { get; set; } = null;
    public string? KSEAccountNumber { get; set; } = null;
    
    /// <summary>
    /// bez foregin key, tylko index
    /// </summary>
    public string DocumentTypeNamePL { get; set; } = "";
    /// <summary>
    /// bez foregin key, tylko index
    /// </summary>
    public int DocumentTypeId { get; set; } = 1;
    
    public string IssuedBy  { get; set; } = "";
    public string? IssuedVatNumber { get; set; } = null;
    public string IssuedFor { get; set; }
    public string IssuedForVatNumber { get; set; } = "";
    
    public decimal VatRate { get; set; } = 0;
}