using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using Identity.Api.Features.Teams.Model;

namespace Identity.Api.Features.Teams;

public interface ITeamService
{
    Task<Result<TeamResponse>> CreateAsync(PortalAuthInfo auth, CreateTeamRequest request);
    Task<Result<TeamResponse>> GetByGidAsync(PortalAuthInfo auth, string gid);
    Task<Result<PagedResult<TeamResponse>>> GetListAsync(PortalAuthInfo auth, TeamListFilter filter);
    Task<Result<TeamResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateTeamRequest request);
    Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid);
}
