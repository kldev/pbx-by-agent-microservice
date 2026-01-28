using App.BaseData;

namespace Rcp.Data.Entities;

/// <summary>
/// Okres rozliczeniowy RCP (miesiąc/rok) - globalny kontener
/// </summary>
public class RcpPeriod : BaseAuditableTable
{
    public int Year { get; set; }
    public int Month { get; set; }

    // Navigation
    public virtual ICollection<RcpMonthlyEntry> MonthlyEntries { get; set; } = [];
}
