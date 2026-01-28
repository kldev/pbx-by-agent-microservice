using Identity.Api.Features.Teams.Model;
using Identity.Data.Entities;

namespace Identity.Api.Features.Teams;

public interface ITeamDataHandler
{
    Task<Team> CreateAsync(Team entity);
    Task<Team?> GetByIdAsync(long id);
    Task<Team?> GetByGidAsync(string gid);
    Task<Team?> GetByCodeAsync(string code);
    Task<(List<Team> Items, int Total)> GetPagedAsync(TeamListFilter filter);
    Task<bool> HasEmployeesAsync(long teamId);
    Task UpdateAsync(Team entity);
    Task SoftDeleteAsync(Team entity);
}
