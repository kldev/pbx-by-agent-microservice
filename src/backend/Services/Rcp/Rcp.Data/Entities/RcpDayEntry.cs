using App.BaseData;

namespace Rcp.Data.Entities;

/// <summary>
/// Wpis czasu dnia pracy pracownika (RCP)
/// </summary>
public class RcpDayEntry : BaseAuditableTable
{
    public long MonthlyEntryId { get; set; }

    /// <summary>Data pracy (MySQL DATE)</summary>
    public DateTime WorkDate { get; set; }

    /// <summary>Godzina rozpoczęcia (MySQL TIME -> TimeSpan)</summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>Godzina zakończenia (MySQL TIME -> TimeSpan)</summary>
    public TimeSpan EndTime { get; set; }

    /// <summary>Czas pracy w minutach (np. 510 = 8h 30m)</summary>
    public int WorkMinutes { get; set; }

    public string? Notes { get; set; }

    // Navigation
    public virtual RcpMonthlyEntry? MonthlyEntry { get; set; }
}
