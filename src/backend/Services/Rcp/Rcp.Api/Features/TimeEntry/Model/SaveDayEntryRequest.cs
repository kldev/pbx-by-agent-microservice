namespace Rcp.Api.Features.TimeEntry.Model;

/// <summary>
/// Request to save a day entry.
/// </summary>
/// <param name="Year">Year (e.g., 2023)</param>
/// <param name="Month">Month (1-12)</param>
/// <param name="WorkDate">Date in format "yyyy-MM-dd"</param>
/// <param name="StartTime">Start time in format "HH:mm"</param>
/// <param name="Hours">Hours worked (0-24)</param>
/// <param name="Minutes">Minutes worked (0, 5, 10, ... 55)</param>
/// <param name="Notes">Optional notes</param>
public record SaveDayEntryRequest(
    int Year,
    int Month,
    string WorkDate,
    string StartTime,
    int Hours,
    int Minutes,
    string? Notes
);
