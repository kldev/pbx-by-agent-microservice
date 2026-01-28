using CdrService.Api.Features.CallRecords.Model;
using CdrService.Data.Entities;

namespace CdrService.Api.Features.CallRecords;

public interface ICallRecordDataHandler
{
    Task<(IEnumerable<CallRecord> Items, int TotalCount)> GetPagedAsync(CallRecordListFilter filter);
    Task<CallRecord?> GetByGidAsync(string gid);
    Task<CallRecord?> GetByGidWithDetailsAsync(string gid);
    Task<bool> CallStatusExistsAsync(int id);
    Task<bool> TerminationCauseExistsAsync(int id);
    Task<bool> CallDirectionExistsAsync(int id);
    Task CreateAsync(CallRecord callRecord);
    Task UpdateAsync(CallRecord callRecord);
    Task DeleteAsync(CallRecord callRecord);
}
