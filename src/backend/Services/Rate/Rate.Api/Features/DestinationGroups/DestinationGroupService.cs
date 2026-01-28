using Common.Toolkit.ResultPattern;
using RateService.Api.Definitions;
using RateService.Api.Features.DestinationGroups.Model;
using RateService.Data.Entities;

namespace RateService.Api.Features.DestinationGroups;

public class DestinationGroupService : IDestinationGroupService
{
    private readonly IDestinationGroupDataHandler _dataHandler;

    public DestinationGroupService(IDestinationGroupDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<Result<IEnumerable<DestinationGroupResponse>>> GetAllAsync()
    {
        var groups = await _dataHandler.GetAllAsync();
        return Result<IEnumerable<DestinationGroupResponse>>.Success(
            groups.Select(MapToResponse));
    }

    public async Task<Result<DestinationGroupResponse>> GetByIdAsync(int id)
    {
        var group = await _dataHandler.GetByIdAsync(id);
        if (group == null)
            return Result<DestinationGroupResponse>.Failure(
                new NotFoundError(ErrorCodes.DestinationGroup.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.DestinationGroup.NotFound)));

        return Result<DestinationGroupResponse>.Success(MapToResponse(group));
    }

    public async Task<Result<DestinationGroupResponse>> CreateAsync(CreateDestinationGroupRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<DestinationGroupResponse>.Failure(
                new ValidationError(ErrorCodes.DestinationGroup.NameRequired, ErrorCodeHelper.GetMessage(ErrorCodes.DestinationGroup.NameRequired)));

        if (await _dataHandler.ExistsByNameAsync(request.Name))
            return Result<DestinationGroupResponse>.Failure(
                new BusinessLogicError(ErrorCodes.DestinationGroup.NameExists, ErrorCodeHelper.GetMessage(ErrorCodes.DestinationGroup.NameExists)));

        var group = new DestinationGroup
        {
            Name = request.Name,
            NamePL = request.NamePL,
            NameEN = request.NameEN,
            Description = request.Description
        };

        await _dataHandler.CreateAsync(group);
        return Result<DestinationGroupResponse>.Success(MapToResponse(group));
    }

    public async Task<Result<DestinationGroupResponse>> UpdateAsync(int id, CreateDestinationGroupRequest request)
    {
        var group = await _dataHandler.GetByIdAsync(id);
        if (group == null)
            return Result<DestinationGroupResponse>.Failure(
                new NotFoundError(ErrorCodes.DestinationGroup.NotFound, ErrorCodeHelper.GetMessage(ErrorCodes.DestinationGroup.NotFound)));

        if (await _dataHandler.ExistsByNameAsync(request.Name, id))
            return Result<DestinationGroupResponse>.Failure(
                new BusinessLogicError(ErrorCodes.DestinationGroup.NameExists, ErrorCodeHelper.GetMessage(ErrorCodes.DestinationGroup.NameExists)));

        group.Name = request.Name;
        group.NamePL = request.NamePL;
        group.NameEN = request.NameEN;
        group.Description = request.Description;

        await _dataHandler.UpdateAsync(group);
        return Result<DestinationGroupResponse>.Success(MapToResponse(group));
    }

    private static DestinationGroupResponse MapToResponse(DestinationGroup group) => new()
    {
        Id = group.Id,
        Name = group.Name,
        NamePL = group.NamePL,
        NameEN = group.NameEN,
        Description = group.Description,
        IsActive = group.IsActive
    };
}
