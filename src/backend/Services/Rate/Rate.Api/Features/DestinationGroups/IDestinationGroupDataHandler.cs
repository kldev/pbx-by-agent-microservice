using RateService.Data.Entities;

namespace RateService.Api.Features.DestinationGroups;

public interface IDestinationGroupDataHandler
{
    Task<IEnumerable<DestinationGroup>> GetAllAsync();
    Task<DestinationGroup?> GetByIdAsync(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task CreateAsync(DestinationGroup group);
    Task UpdateAsync(DestinationGroup group);
}
