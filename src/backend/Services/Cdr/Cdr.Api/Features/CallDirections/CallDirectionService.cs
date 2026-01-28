using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using CdrService.Api.Definitions;
using CdrService.Api.Features.CallDirections.Model;
using CdrService.Data.Entities;

namespace CdrService.Api.Features.CallDirections;

public class CallDirectionService : ICallDirectionService
{
    private readonly ICallDirectionDataHandler _dataHandler;

    public CallDirectionService(ICallDirectionDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<Result<IEnumerable<CallDirectionResponse>>> GetListAsync(PortalAuthInfo auth)
    {
        var items = await _dataHandler.GetAllActiveAsync();
        return Result<IEnumerable<CallDirectionResponse>>.Success(items.Select(MapToResponse));
    }

    public async Task<Result<CallDirectionResponse>> GetByGidAsync(PortalAuthInfo auth, string gid)
    {
        var item = await _dataHandler.GetByGidAsync(gid);
        if (item == null)
            return Result<CallDirectionResponse>.Failure(
                new NotFoundError(ErrorCodes.CallDirection.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.CallDirection.NotFound)));

        return Result<CallDirectionResponse>.Success(MapToResponse(item));
    }

    private static CallDirectionResponse MapToResponse(CallDirection d) => new()
    {
        Gid = d.Gid,
        Code = d.Code,
        NamePL = d.NamePL,
        NameEN = d.NameEN,
        Description = d.Description,
        SortOrder = d.SortOrder
    };
}
