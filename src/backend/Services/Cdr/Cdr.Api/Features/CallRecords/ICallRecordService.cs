using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using CdrService.Api.Features.CallRecords.Model;

namespace CdrService.Api.Features.CallRecords;

public interface ICallRecordService
{
    Task<Result<PagedResult<CallRecordResponse>>> GetListAsync(PortalAuthInfo auth, CallRecordListFilter filter);
    Task<Result<CallRecordDetailResponse>> GetByGidAsync(PortalAuthInfo auth, string gid);
    Task<Result<CallRecordResponse>> CreateAsync(PortalAuthInfo auth, CreateCallRecordRequest request);
    Task<Result<CallRecordResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateCallRecordRequest request);
    Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid);
}
