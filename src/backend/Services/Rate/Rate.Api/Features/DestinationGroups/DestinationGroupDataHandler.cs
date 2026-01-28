using Microsoft.EntityFrameworkCore;
using RateService.Data;
using RateService.Data.Entities;

namespace RateService.Api.Features.DestinationGroups;

public class DestinationGroupDataHandler : IDestinationGroupDataHandler
{
    private readonly RateServiceDbContext _context;

    public DestinationGroupDataHandler(RateServiceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DestinationGroup>> GetAllAsync()
    {
        return await _context.DestinationGroups
            .Where(g => g.IsActive)
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<DestinationGroup?> GetByIdAsync(int id)
    {
        return await _context.DestinationGroups.FindAsync(id);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _context.DestinationGroups.Where(g => g.Name == name);
        if (excludeId.HasValue)
            query = query.Where(g => g.Id != excludeId);
        return await query.AnyAsync();
    }

    public async Task CreateAsync(DestinationGroup group)
    {
        _context.DestinationGroups.Add(group);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DestinationGroup group)
    {
        _context.DestinationGroups.Update(group);
        await _context.SaveChangesAsync();
    }
}
