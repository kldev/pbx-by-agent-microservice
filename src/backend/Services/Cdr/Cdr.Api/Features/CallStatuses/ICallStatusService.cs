using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using CdrService.Api.Features.CallStatuses.Model;

namespace CdrService.Api.Features.CallStatuses;

public interface ICallStatusService
{
    Task<Result<IEnumerable<CallStatusResponse>>> GetListAsync(PortalAuthInfo auth);
    Task<Result<CallStatusResponse>> GetByGidAsync(PortalAuthInfo auth, string gid);
}
