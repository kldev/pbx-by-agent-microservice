namespace Rcp.Api.Features.TimeEntry.Model;

/// <summary>
/// Day entry response.
/// </summary>
/// <param name="Gid">Global ID</param>
/// <param name="WorkDate">Date in format "yyyy-MM-dd"</param>
/// <param name="StartTime">Start time in format "HH:mm"</param>
/// <param name="EndTime">End time in format "HH:mm"</param>
/// <param name="WorkMinutes">Total work time in minutes</param>
/// <param name="Hours">Hours portion</param>
/// <param name="Minutes">Minutes portion</param>
/// <param name="TimeFormatted">Formatted time (e.g., "8:30")</param>
/// <param name="DecimalHours">Decimal hours (e.g., 8.5)</param>
/// <param name="Notes">Optional notes</param>
public record DayEntryResponse(
    string Gid,
    string WorkDate,
    string StartTime,
    string EndTime,
    int WorkMinutes,
    int Hours,
    int Minutes,
    string TimeFormatted,
    decimal DecimalHours,
    string? Notes
);
