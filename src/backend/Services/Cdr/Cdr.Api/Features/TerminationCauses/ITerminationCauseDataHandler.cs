using CdrService.Data.Entities;

namespace CdrService.Api.Features.TerminationCauses;

public interface ITerminationCauseDataHandler
{
    Task<IEnumerable<TerminationCause>> GetAllActiveAsync();
    Task<TerminationCause?> GetByGidAsync(string gid);
}
