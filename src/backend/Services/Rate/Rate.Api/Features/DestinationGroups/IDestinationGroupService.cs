using Common.Toolkit.ResultPattern;
using RateService.Api.Features.DestinationGroups.Model;

namespace RateService.Api.Features.DestinationGroups;

public interface IDestinationGroupService
{
    Task<Result<IEnumerable<DestinationGroupResponse>>> GetAllAsync();
    Task<Result<DestinationGroupResponse>> GetByIdAsync(int id);
    Task<Result<DestinationGroupResponse>> CreateAsync(CreateDestinationGroupRequest request);
    Task<Result<DestinationGroupResponse>> UpdateAsync(int id, CreateDestinationGroupRequest request);
}
