using App.BaseData;

namespace RateService.Data.Entities;

/// <summary>
/// Stawka za połączenie dla danego prefiksu
/// </summary>
public class Rate : BaseAuditableTable
{
    /// <summary>
    /// Prefiks numeru (np. +48, +4930, +1212)
    /// </summary>
    public string Prefix { get; set; } = null!;

    /// <summary>
    /// Nazwa destynacji (np. "Poland Mobile", "Germany Berlin")
    /// </summary>
    public string DestinationName { get; set; } = null!;

    /// <summary>
    /// Stawka za minutę
    /// </summary>
    public decimal RatePerMinute { get; set; }

    /// <summary>
    /// Opłata za połączenie (nadpisuje taryfę jeśli ustawiona)
    /// </summary>
    public decimal? ConnectionFee { get; set; }

    /// <summary>
    /// Interwał naliczania (nadpisuje taryfę jeśli ustawiony)
    /// </summary>
    public int? BillingIncrement { get; set; }

    /// <summary>
    /// Min. długość (nadpisuje taryfę jeśli ustawiona)
    /// </summary>
    public int? MinimumDuration { get; set; }

    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // FK
    public long TariffId { get; set; }
    public Tariff? Tariff { get; set; }

    public int? DestinationGroupId { get; set; }
    public DestinationGroup? DestinationGroup { get; set; }
}
