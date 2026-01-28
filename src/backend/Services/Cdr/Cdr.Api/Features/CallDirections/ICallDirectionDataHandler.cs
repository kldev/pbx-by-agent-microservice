using CdrService.Data.Entities;

namespace CdrService.Api.Features.CallDirections;

public interface ICallDirectionDataHandler
{
    Task<IEnumerable<CallDirection>> GetAllActiveAsync();
    Task<CallDirection?> GetByGidAsync(string gid);
}
