namespace Rcp.Api.Features.TimeEntry.Model;

public record GetPeriodRequest(int Year, int Month);

public record SubmitRequest(int Year, int Month, string? Comment);
