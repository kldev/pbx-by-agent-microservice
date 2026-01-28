using Microsoft.EntityFrameworkCore;
using CdrService.Api.Features.CallRecords.Model;
using CdrService.Data;
using CdrService.Data.Entities;

namespace CdrService.Api.Features.CallRecords;

public class CallRecordDataHandler : ICallRecordDataHandler
{
    private readonly CdrDbContext _context;

    public CallRecordDataHandler(CdrDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<CallRecord> Items, int TotalCount)> GetPagedAsync(CallRecordListFilter filter)
    {
        var query = _context.CallRecords
            .Include(c => c.CallStatus)
            .Include(c => c.CallDirection)
            .Where(c => !c.IsDeleted)
            .AsQueryable();

        // Filtrowanie
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(c =>
                c.CallerId.ToLower().Contains(search) ||
                c.CalledNumber.ToLower().Contains(search));
        }

        if (filter.StartDateFrom.HasValue)
            query = query.Where(c => c.StartTime >= filter.StartDateFrom.Value);

        if (filter.StartDateTo.HasValue)
            query = query.Where(c => c.StartTime <= filter.StartDateTo.Value);

        if (filter.CallStatusId.HasValue)
            query = query.Where(c => c.CallStatusId == filter.CallStatusId.Value);

        if (filter.CallDirectionId.HasValue)
            query = query.Where(c => c.CallDirectionId == filter.CallDirectionId.Value);

        if (!string.IsNullOrWhiteSpace(filter.CustomerGid))
            query = query.Where(c => c.CustomerGid == filter.CustomerGid);

        if (!string.IsNullOrWhiteSpace(filter.SipAccountGid))
            query = query.Where(c => c.SipAccountGid == filter.SipAccountGid);

        if (filter.MinCost.HasValue)
            query = query.Where(c => c.TotalCost >= filter.MinCost.Value);

        if (filter.MaxCost.HasValue)
            query = query.Where(c => c.TotalCost <= filter.MaxCost.Value);

        // Liczenie
        var totalCount = await query.CountAsync();

        // Paginacja
        var items = await query
            .OrderByDescending(c => c.StartTime)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<CallRecord?> GetByGidAsync(string gid)
    {
        return await _context.CallRecords
            .FirstOrDefaultAsync(c => c.Gid == gid && !c.IsDeleted);
    }

    public async Task<CallRecord?> GetByGidWithDetailsAsync(string gid)
    {
        return await _context.CallRecords
            .Include(c => c.CallStatus)
            .Include(c => c.TerminationCause)
            .Include(c => c.CallDirection)
            .FirstOrDefaultAsync(c => c.Gid == gid && !c.IsDeleted);
    }

    public async Task<bool> CallStatusExistsAsync(int id)
    {
        return await _context.CallStatuses.AnyAsync(s => s.Id == id && s.IsActive);
    }

    public async Task<bool> TerminationCauseExistsAsync(int id)
    {
        return await _context.TerminationCauses.AnyAsync(t => t.Id == id && t.IsActive);
    }

    public async Task<bool> CallDirectionExistsAsync(int id)
    {
        return await _context.CallDirections.AnyAsync(d => d.Id == id && d.IsActive);
    }

    public async Task CreateAsync(CallRecord callRecord)
    {
        _context.CallRecords.Add(callRecord);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CallRecord callRecord)
    {
        _context.CallRecords.Update(callRecord);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(CallRecord callRecord)
    {
        callRecord.IsDeleted = true;
        callRecord.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
