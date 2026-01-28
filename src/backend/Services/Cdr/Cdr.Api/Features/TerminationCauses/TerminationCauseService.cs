using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using CdrService.Api.Definitions;
using CdrService.Api.Features.TerminationCauses.Model;
using CdrService.Data.Entities;

namespace CdrService.Api.Features.TerminationCauses;

public class TerminationCauseService : ITerminationCauseService
{
    private readonly ITerminationCauseDataHandler _dataHandler;

    public TerminationCauseService(ITerminationCauseDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<Result<IEnumerable<TerminationCauseResponse>>> GetListAsync(PortalAuthInfo auth)
    {
        var items = await _dataHandler.GetAllActiveAsync();
        return Result<IEnumerable<TerminationCauseResponse>>.Success(items.Select(MapToResponse));
    }

    public async Task<Result<TerminationCauseResponse>> GetByGidAsync(PortalAuthInfo auth, string gid)
    {
        var item = await _dataHandler.GetByGidAsync(gid);
        if (item == null)
            return Result<TerminationCauseResponse>.Failure(
                new NotFoundError(ErrorCodes.TerminationCause.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.TerminationCause.NotFound)));

        return Result<TerminationCauseResponse>.Success(MapToResponse(item));
    }

    private static TerminationCauseResponse MapToResponse(TerminationCause t) => new()
    {
        Gid = t.Gid,
        Code = t.Code,
        Q850Code = t.Q850Code,
        NamePL = t.NamePL,
        NameEN = t.NameEN,
        Description = t.Description,
        SortOrder = t.SortOrder
    };
}
