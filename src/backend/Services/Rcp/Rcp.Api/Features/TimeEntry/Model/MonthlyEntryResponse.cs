using App.Bps.Enum.Rcp;

namespace Rcp.Api.Features.TimeEntry.Model;

public record MonthlyEntryResponse(
    string Gid,
    int Year,
    int Month,
    RcpTimeEntryStatus Status,
    int TotalMinutes,
    string TotalFormatted,
    decimal TotalDecimalHours,
    string? EmployeeFullName,
    string? UserGid,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    string? ApprovedByFullName,
    List<DayEntryResponse> Days,
    List<CommentResponse> Comments
);
