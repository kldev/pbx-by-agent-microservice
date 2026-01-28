using Microsoft.EntityFrameworkCore;
using CdrService.Data;
using CdrService.Data.Entities;

namespace CdrService.Api.Features.CallStatuses;

public class CallStatusDataHandler : ICallStatusDataHandler
{
    private readonly CdrDbContext _context;

    public CallStatusDataHandler(CdrDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CallStatus>> GetAllActiveAsync()
    {
        return await _context.CallStatuses
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder)
            .ToListAsync();
    }

    public async Task<CallStatus?> GetByGidAsync(string gid)
    {
        return await _context.CallStatuses
            .FirstOrDefaultAsync(s => s.Gid == gid && s.IsActive);
    }
}
