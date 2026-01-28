using App.BaseData;

namespace RateService.Data.Entities;

/// <summary>
/// Taryfa/Cennik - zbiór stawek za połączenia
/// </summary>
public class Tariff : BaseAuditableTable
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CurrencyCode { get; set; } = "PLN";
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// Interwał naliczania w sekundach (np. 60, 30, 1)
    /// </summary>
    public int BillingIncrement { get; set; } = 60;

    /// <summary>
    /// Minimalna długość naliczana (sekundy)
    /// </summary>
    public int MinimumDuration { get; set; } = 0;

    /// <summary>
    /// Opłata za połączenie (setup fee)
    /// </summary>
    public decimal ConnectionFee { get; set; } = 0;

    // Navigation
    public ICollection<Rate> Rates { get; set; } = new List<Rate>();
}
