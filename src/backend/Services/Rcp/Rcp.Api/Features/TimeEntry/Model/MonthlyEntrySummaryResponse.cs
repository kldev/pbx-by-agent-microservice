using App.Bps.Enum.Rcp;

namespace Rcp.Api.Features.TimeEntry.Model;

public record MonthlyEntrySummaryResponse(
    string Gid,
    string? EmployeeFullName,
    string? UserGid,
    RcpTimeEntryStatus Status,
    int TotalMinutes,
    string TotalFormatted,
    DateTime? SubmittedAt
);
