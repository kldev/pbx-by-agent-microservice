using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using Identity.Api.Features.AppUsers.Model;

namespace Identity.Api.Features.AppUsers;

public interface IAppUserService
{
    Task<Result<AppUserResponse>> CreateAsync(PortalAuthInfo auth, CreateAppUserRequest request);
    Task<Result<AppUserResponse>> GetByGidAsync(PortalAuthInfo auth, string gid);
    Task<Result<PagedResult<AppUserResponse>>> GetListAsync(PortalAuthInfo auth, AppUserListFilter filter);
    Task<Result<AppUserResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateAppUserRequest request);
    Task<Result<bool>> ChangePasswordAsync(PortalAuthInfo auth, string gid, ChangePasswordRequest request);
    Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid);
    Task<Result<AppUserResponse>> SetSupervisorAsync(PortalAuthInfo auth, string gid, SetSupervisorRequest request);
}
