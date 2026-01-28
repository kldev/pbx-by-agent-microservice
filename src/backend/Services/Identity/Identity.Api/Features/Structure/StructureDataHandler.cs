using Identity.Api.Features.Structure.Model;
using Identity.Data;
using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Features.Structure;

public class StructureDataHandler : IStructureDataHandler
{
    private readonly IdentityDbContext _context;

    public StructureDataHandler(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<StructureDict> CreateAsync(StructureDict entity)
    {
        // StructureDict uses ValueGeneratedNever, so we need to generate the ID
        var maxId = await _context.StructureDict.MaxAsync(x => (int?)x.Id) ?? 0;
        entity.Id = maxId + 1;

        _context.StructureDict.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<StructureDict?> GetByIdAsync(int id)
    {
        return await _context.StructureDict.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<StructureDict?> GetByCodeAsync(string code)
    {
        return await _context.StructureDict.FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<List<StructureDict>> GetAllAsync(bool? isActive = null)
    {
        var query = _context.StructureDict.AsQueryable();

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        return await query.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<(List<StructureDict> Items, int Total)> GetPagedAsync(StructureListFilter filter)
    {
        var query = _context.StructureDict.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(x =>
                x.Code.ToLower().Contains(search) ||
                x.Name.ToLower().Contains(search));
        }

        if (filter.Region.HasValue)
        {
            query = query.Where(x => x.Region == filter.Region.Value);
        }

        if (filter.IsActive.HasValue)
            query = query.Where(x => x.IsActive == filter.IsActive.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(x => x.Name)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<bool> HasTeamsAsync(int structureId)
    {
        return await _context.Teams.AnyAsync(x => x.StructureId == structureId && x.IsActive);
    }

    public async Task UpdateAsync(StructureDict entity)
    {
        _context.StructureDict.Update(entity);
        await _context.SaveChangesAsync();
    }
}
