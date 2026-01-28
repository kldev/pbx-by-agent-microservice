using Microsoft.EntityFrameworkCore;
using CdrService.Data;
using CdrService.Data.Entities;

namespace CdrService.Api.Features.TerminationCauses;

public class TerminationCauseDataHandler : ITerminationCauseDataHandler
{
    private readonly CdrDbContext _context;

    public TerminationCauseDataHandler(CdrDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TerminationCause>> GetAllActiveAsync()
    {
        return await _context.TerminationCauses
            .Where(t => t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();
    }

    public async Task<TerminationCause?> GetByGidAsync(string gid)
    {
        return await _context.TerminationCauses
            .FirstOrDefaultAsync(t => t.Gid == gid && t.IsActive);
    }
}
