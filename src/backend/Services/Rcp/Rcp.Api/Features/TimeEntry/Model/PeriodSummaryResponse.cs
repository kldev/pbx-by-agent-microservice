namespace Rcp.Api.Features.TimeEntry.Model;

public record PeriodSummaryResponse(
    int Year,
    int Month,
    List<MonthlyEntrySummaryResponse> Entries
);
