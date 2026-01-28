using App.Bps.Enum.Rcp;

namespace Rcp.Api.Features.TimeEntry.Model;

/// <summary>
/// Response for supervisor monitoring - shows ALL entries (all statuses) for subordinates.
/// Allows supervisor to track progress before formal submission.
/// </summary>
public record MonitorPeriodResponse(
    int Year,
    int Month,
    int WorkingDaysInMonth,
    List<MonitorEntrySummary> Entries
);

/// <summary>
/// Summary of a subordinate's time entry for monitoring purposes.
/// </summary>
public record MonitorEntrySummary(
    /// <summary>Entry GID (null if no entry exists yet)</summary>
    string? Gid,

    /// <summary>User ID of the subordinate</summary>
    long UserId,

    /// <summary>Full name of the subordinate</summary>
    string? UserFullName,

    /// <summary>Current status of the entry</summary>
    RcpTimeEntryStatus Status,

    /// <summary>Total hours worked (decimal format)</summary>
    decimal TotalHours,

    /// <summary>Number of days with entries</summary>
    int FilledDays,

    /// <summary>Date of the last entry (format: yyyy-MM-dd)</summary>
    string? LastEntryDate
);
