using App.BaseData;
using App.Bps.Enum.Rcp;

namespace Rcp.Data.Entities;

/// <summary>
/// Wpis miesięczny pracownika (RCP) - jeden per pracownik per okres
/// </summary>
public class RcpMonthlyEntry : BaseAuditableTable
{
    public long RcpPeriodId { get; set; }

    /// <summary>ID użytkownika (z Identity service)</summary>
    public long UserId { get; set; }

    /// <summary>GID użytkownika (z Identity service)</summary>
    public string? UserGid { get; set; }

    public RcpTimeEntryStatus Status { get; set; } = RcpTimeEntryStatus.Draft;

    /// <summary>Suma minut (przeliczane z DayEntries)</summary>
    public int TotalMinutes { get; set; }

    /// <summary>Snapshot imienia i nazwiska pracownika</summary>
    public string? EmployeeFullName { get; set; }

    // Status change tracking
    public DateTime? StatusChangedAt { get; set; }
    public long? StatusChangedByUserId { get; set; }
    public string? StatusChangedByFullName { get; set; }

    // Submit tracking
    public DateTime? SubmittedAt { get; set; }

    // Approve tracking
    public DateTime? ApprovedAt { get; set; }
    public long? ApprovedByUserId { get; set; }
    public string? ApprovedByFullName { get; set; }

    // Navigation
    public virtual RcpPeriod? RcpPeriod { get; set; }
    public virtual ICollection<RcpDayEntry> DayEntries { get; set; } = [];
    public virtual ICollection<RcpEntryComment> Comments { get; set; } = [];
}
