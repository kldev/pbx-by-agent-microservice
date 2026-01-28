using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using CdrService.Api.Features.CallDirections.Model;

namespace CdrService.Api.Features.CallDirections;

public interface ICallDirectionService
{
    Task<Result<IEnumerable<CallDirectionResponse>>> GetListAsync(PortalAuthInfo auth);
    Task<Result<CallDirectionResponse>> GetByGidAsync(PortalAuthInfo auth, string gid);
}
