using Identity.Api.Features.Teams.Model;
using Identity.Data;
using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Features.Teams;

public class TeamDataHandler : ITeamDataHandler
{
    private readonly IdentityDbContext _context;

    public TeamDataHandler(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Team> CreateAsync(Team entity)
    {
        _context.Teams.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Team?> GetByIdAsync(long id)
    {
        return await _context.TeamsWithStructure
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Team?> GetByGidAsync(string gid)
    {
        return await _context.TeamsWithStructure
            .FirstOrDefaultAsync(x => x.Gid == gid);
    }

    public async Task<Team?> GetByCodeAsync(string code)
    {
        return await _context.Teams.FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<(List<Team> Items, int Total)> GetPagedAsync(TeamListFilter filter)
    {
        var query = _context.TeamsWithStructure.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(x =>
                x.Code.ToLower().Contains(search) ||
                x.Name.ToLower().Contains(search));
        }

        if (filter.StructureId.HasValue)
            query = query.Where(x => x.StructureId == filter.StructureId.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(x => x.IsActive == filter.IsActive.Value);

        if (filter.Type.HasValue)
            query = query.Where(x => x.Type == filter.Type.Value);

        // Count bez Include (wydajność)
        var countQuery = _context.Teams.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            countQuery = countQuery.Where(x =>
                x.Code.ToLower().Contains(search) ||
                x.Name.ToLower().Contains(search));
        }
        if (filter.StructureId.HasValue)
            countQuery = countQuery.Where(x => x.StructureId == filter.StructureId.Value);
        if (filter.IsActive.HasValue)
            countQuery = countQuery.Where(x => x.IsActive == filter.IsActive.Value);
        if (filter.Type.HasValue)
            countQuery = countQuery.Where(x => x.Type == filter.Type.Value);

        var total = await countQuery.CountAsync();

        var items = await query
            .OrderBy(x => x.StructureId)
            .ThenBy(x => x.Name)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<bool> HasEmployeesAsync(long teamId)
    {
        return await _context.Employees.AnyAsync(x => x.TeamId == teamId && !x.IsDeleted);
    }

    public async Task UpdateAsync(Team entity)
    {
        entity.ModifiedAt = DateTime.UtcNow;
        _context.Teams.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Team entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
