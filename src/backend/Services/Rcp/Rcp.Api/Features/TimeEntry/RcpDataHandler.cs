using Microsoft.EntityFrameworkCore;
using Rcp.Data;
using Rcp.Data.Entities;

namespace Rcp.Api.Features.TimeEntry;

public class RcpDataHandler : IRcpDataHandler
{
    private readonly RcpDbContext _context;

    public RcpDataHandler(RcpDbContext context)
    {
        _context = context;
    }

    #region Period

    public async Task<RcpPeriod?> GetPeriodAsync(int year, int month)
    {
        return await _context.RcpPeriods
            .FirstOrDefaultAsync(p => p.Year == year && p.Month == month);
    }

    public async Task<RcpPeriod> GetOrCreatePeriodAsync(int year, int month)
    {
        var period = await GetPeriodAsync(year, month);
        if (period != null)
            return period;

        period = new RcpPeriod
        {
            Gid = Guid.NewGuid().ToString(),
            Year = year,
            Month = month
        };

        _context.RcpPeriods.Add(period);
        await _context.SaveChangesAsync();
        return period;
    }

    #endregion

    #region Monthly Entry

    public async Task<RcpMonthlyEntry?> GetMonthlyEntryAsync(long periodId, long userId)
    {
        return await _context.RcpMonthlyEntries
            .Include(e => e.DayEntries.Where(d => !d.IsDeleted))
            .Include(e => e.Comments.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(e => e.RcpPeriodId == periodId && e.UserId == userId);
    }

    public async Task<RcpMonthlyEntry?> GetMonthlyEntryByGidAsync(string gid)
    {
        return await _context.RcpMonthlyEntries
            .Include(e => e.RcpPeriod)
            .Include(e => e.DayEntries.Where(d => !d.IsDeleted))
            .Include(e => e.Comments.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(e => e.Gid == gid);
    }

    public async Task<RcpMonthlyEntry> CreateMonthlyEntryAsync(RcpMonthlyEntry entry)
    {
        _context.RcpMonthlyEntries.Add(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task UpdateMonthlyEntryAsync(RcpMonthlyEntry entry)
    {
        _context.RcpMonthlyEntries.Update(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<List<RcpMonthlyEntry>> GetMonthlyEntriesForPeriodAsync(long periodId)
    {
        return await _context.RcpMonthlyEntries
            .Include(e => e.DayEntries.Where(d => !d.IsDeleted))
            .Where(e => e.RcpPeriodId == periodId)
            .OrderBy(e => e.EmployeeFullName)
            .ToListAsync();
    }

    #endregion

    #region Day Entry

    public async Task<RcpDayEntry?> GetDayEntryAsync(long monthlyEntryId, DateTime workDate)
    {
        // Compare date part only
        return await _context.RcpDayEntries
            .FirstOrDefaultAsync(d => d.MonthlyEntryId == monthlyEntryId && d.WorkDate.Date == workDate.Date);
    }

    public async Task<RcpDayEntry> CreateDayEntryAsync(RcpDayEntry entry)
    {
        _context.RcpDayEntries.Add(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task UpdateDayEntryAsync(RcpDayEntry entry)
    {
        _context.RcpDayEntries.Update(entry);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDayEntryAsync(RcpDayEntry entry)
    {
        entry.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Comments

    public async Task<RcpEntryComment> CreateCommentAsync(RcpEntryComment comment)
    {
        _context.RcpEntryComments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<List<RcpEntryComment>> GetCommentsAsync(long monthlyEntryId)
    {
        return await _context.RcpEntryComments
            .Where(c => c.MonthlyEntryId == monthlyEntryId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    #endregion

    #region Calculations

    public async Task<int> CalculateTotalMinutesAsync(long monthlyEntryId)
    {
        return await _context.RcpDayEntries
            .Where(d => d.MonthlyEntryId == monthlyEntryId && !d.IsDeleted)
            .SumAsync(d => d.WorkMinutes);
    }

    #endregion
}
