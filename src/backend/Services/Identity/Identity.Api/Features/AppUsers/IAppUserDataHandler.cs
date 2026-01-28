using Identity.Api.Features.AppUsers.Model;
using Identity.Data.Entities;

namespace Identity.Api.Features.AppUsers;

public interface IAppUserDataHandler
{
    Task<AppUser> CreateAsync(AppUser entity);
    Task<AppUser?> GetByIdAsync(long id);
    Task<AppUser?> GetByGidAsync(string gid);
    Task<AppUser?> GetByGidWithRolesAsync(string gid);
    Task<AppUser?> GetByGidWithSbuAndTeamAsync(string gid);
    Task<AppUser?> GetByEmailAsync(string email);
    Task<(List<AppUser> Items, int Total)> GetPagedAsync(AppUserListFilter filter);
    Task UpdateAsync(AppUser entity);
    Task SoftDeleteAsync(AppUser entity);
}
