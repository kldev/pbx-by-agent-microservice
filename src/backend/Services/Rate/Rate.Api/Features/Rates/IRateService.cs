using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using RateService.Api.Features.Rates.Model;

namespace RateService.Api.Features.Rates;

public interface IRateService
{
    Task<Result<PagedResult<RateResponse>>> GetListAsync(PortalAuthInfo auth, RateListFilter filter);
    Task<Result<RateResponse>> GetByGidAsync(PortalAuthInfo auth, string gid);
    Task<Result<RateResponse>> CreateAsync(PortalAuthInfo auth, CreateRateRequest request);
    Task<Result<RateResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateRateRequest request);
    Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid);
    Task<Result<RateResponse>> FindRateForNumberAsync(PortalAuthInfo auth, string tariffGid, string phoneNumber);
}
