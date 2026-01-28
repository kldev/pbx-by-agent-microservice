using Microsoft.EntityFrameworkCore;
using CdrService.Data;
using CdrService.Data.Entities;

namespace CdrService.Api.Features.CallDirections;

public class CallDirectionDataHandler : ICallDirectionDataHandler
{
    private readonly CdrDbContext _context;

    public CallDirectionDataHandler(CdrDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CallDirection>> GetAllActiveAsync()
    {
        return await _context.CallDirections
            .Where(d => d.IsActive)
            .OrderBy(d => d.SortOrder)
            .ToListAsync();
    }

    public async Task<CallDirection?> GetByGidAsync(string gid)
    {
        return await _context.CallDirections
            .FirstOrDefaultAsync(d => d.Gid == gid && d.IsActive);
    }
}
