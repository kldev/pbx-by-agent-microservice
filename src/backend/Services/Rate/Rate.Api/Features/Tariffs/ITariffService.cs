using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using RateService.Api.Features.Tariffs.Model;

namespace RateService.Api.Features.Tariffs;

public interface ITariffService
{
    Task<Result<PagedResult<TariffResponse>>> GetListAsync(PortalAuthInfo auth, TariffListFilter filter);
    Task<Result<TariffDetailResponse>> GetByGidAsync(PortalAuthInfo auth, string gid);
    Task<Result<TariffResponse>> CreateAsync(PortalAuthInfo auth, CreateTariffRequest request);
    Task<Result<TariffResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateTariffRequest request);
    Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid);
}
