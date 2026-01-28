using Rcp.Data.Entities;

namespace Rcp.Api.Features.TimeEntry;

public interface IRcpDataHandler
{
    // Period
    Task<RcpPeriod?> GetPeriodAsync(int year, int month);
    Task<RcpPeriod> GetOrCreatePeriodAsync(int year, int month);

    // Monthly Entry
    Task<RcpMonthlyEntry?> GetMonthlyEntryAsync(long periodId, long userId);
    Task<RcpMonthlyEntry?> GetMonthlyEntryByGidAsync(string gid);
    Task<RcpMonthlyEntry> CreateMonthlyEntryAsync(RcpMonthlyEntry entry);
    Task UpdateMonthlyEntryAsync(RcpMonthlyEntry entry);
    Task<List<RcpMonthlyEntry>> GetMonthlyEntriesForPeriodAsync(long periodId);

    // Day Entry
    Task<RcpDayEntry?> GetDayEntryAsync(long monthlyEntryId, DateTime workDate);
    Task<RcpDayEntry> CreateDayEntryAsync(RcpDayEntry entry);
    Task UpdateDayEntryAsync(RcpDayEntry entry);
    Task DeleteDayEntryAsync(RcpDayEntry entry);

    // Comments
    Task<RcpEntryComment> CreateCommentAsync(RcpEntryComment comment);
    Task<List<RcpEntryComment>> GetCommentsAsync(long monthlyEntryId);

    // Calculations
    Task<int> CalculateTotalMinutesAsync(long monthlyEntryId);
}
