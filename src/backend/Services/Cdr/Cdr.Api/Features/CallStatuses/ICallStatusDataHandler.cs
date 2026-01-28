using CdrService.Data.Entities;

namespace CdrService.Api.Features.CallStatuses;

public interface ICallStatusDataHandler
{
    Task<IEnumerable<CallStatus>> GetAllActiveAsync();
    Task<CallStatus?> GetByGidAsync(string gid);
}
