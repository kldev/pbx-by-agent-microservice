using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using CdrService.Api.Features.TerminationCauses.Model;

namespace CdrService.Api.Features.TerminationCauses;

public interface ITerminationCauseService
{
    Task<Result<IEnumerable<TerminationCauseResponse>>> GetListAsync(PortalAuthInfo auth);
    Task<Result<TerminationCauseResponse>> GetByGidAsync(PortalAuthInfo auth, string gid);
}
