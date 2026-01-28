using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using CdrService.Api.Definitions;
using CdrService.Api.Features.CallStatuses.Model;
using CdrService.Data.Entities;

namespace CdrService.Api.Features.CallStatuses;

public class CallStatusService : ICallStatusService
{
    private readonly ICallStatusDataHandler _dataHandler;

    public CallStatusService(ICallStatusDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<Result<IEnumerable<CallStatusResponse>>> GetListAsync(PortalAuthInfo auth)
    {
        var items = await _dataHandler.GetAllActiveAsync();
        return Result<IEnumerable<CallStatusResponse>>.Success(items.Select(MapToResponse));
    }

    public async Task<Result<CallStatusResponse>> GetByGidAsync(PortalAuthInfo auth, string gid)
    {
        var item = await _dataHandler.GetByGidAsync(gid);
        if (item == null)
            return Result<CallStatusResponse>.Failure(
                new NotFoundError(ErrorCodes.CallStatus.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.CallStatus.NotFound)));

        return Result<CallStatusResponse>.Success(MapToResponse(item));
    }

    private static CallStatusResponse MapToResponse(CallStatus s) => new()
    {
        Gid = s.Gid,
        Code = s.Code,
        NamePL = s.NamePL,
        NameEN = s.NameEN,
        Description = s.Description,
        SortOrder = s.SortOrder
    };
}
