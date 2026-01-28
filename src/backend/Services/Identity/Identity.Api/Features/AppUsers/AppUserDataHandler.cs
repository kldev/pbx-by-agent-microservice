using Identity.Api.Features.AppUsers.Model;
using Identity.Data;
using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Features.AppUsers;

public class AppUserDataHandler : IAppUserDataHandler
{
    private readonly IdentityDbContext _context;

    public AppUserDataHandler(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<AppUser> CreateAsync(AppUser entity)
    {
        _context.Employees.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<AppUser?> GetByIdAsync(long id)
    {
        return await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<AppUser?> GetByGidAsync(string gid)
    {
        return await _context.Employees.FirstOrDefaultAsync(x => x.Gid == gid);
    }

    public async Task<AppUser?> GetByGidWithRolesAsync(string gid)
    {
        return await _context.EmployeesWithRoles
            .FirstOrDefaultAsync(x => x.Gid == gid);
    }

    public async Task<AppUser?> GetByGidWithSbuAndTeamAsync(string gid)
    {
        return await _context.EmployeesWithOrg
            .FirstOrDefaultAsync(x => x.Gid == gid);
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _context.EmployeesWithRoles
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<(List<AppUser> Items, int Total)> GetPagedAsync(AppUserListFilter filter)
    {
        var query = _context.EmployeesWithRoles;

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(x =>
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search));
        }

        if (filter.IsActive.HasValue)
            query = query.Where(x => x.IsActive == filter.IsActive.Value);

        if (filter.Department.HasValue)
            query = query.Where(x => x.Department == filter.Department.Value);

        if (filter.StructureId.HasValue)
            query = query.Where(x => x.StructureId == filter.StructureId.Value);

        if (filter.TeamId.HasValue)
            query = query.Where(x => x.TeamId == filter.TeamId.Value);

        // Count bez Include (wydajność)
        var countQuery = _context.Employees.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            countQuery = countQuery.Where(x =>
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search));
        }
        if (filter.IsActive.HasValue)
            countQuery = countQuery.Where(x => x.IsActive == filter.IsActive.Value);
        if (filter.Department.HasValue)
            countQuery = countQuery.Where(x => x.Department == filter.Department.Value);
        if (filter.StructureId.HasValue)
            countQuery = countQuery.Where(x => x.StructureId == filter.StructureId.Value);
        if (filter.TeamId.HasValue)
            countQuery = countQuery.Where(x => x.TeamId == filter.TeamId.Value);

        var total = await countQuery.CountAsync();

        var items = await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task UpdateAsync(AppUser entity)
    {
        entity.ModifiedAt = DateTime.UtcNow;
        _context.Employees.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(AppUser entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
