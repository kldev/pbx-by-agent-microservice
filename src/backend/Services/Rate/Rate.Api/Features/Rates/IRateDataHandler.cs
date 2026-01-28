using RateService.Api.Features.Rates.Model;
using RateService.Data.Entities;

namespace RateService.Api.Features.Rates;

public interface IRateDataHandler
{
    Task<(IEnumerable<Rate> Items, int TotalCount)> GetPagedAsync(RateListFilter filter);
    Task<Rate?> GetByGidAsync(string gid);
    Task<bool> ExistsByPrefixInTariffAsync(long tariffId, string prefix, long? excludeId = null);
    Task<Rate?> FindRateForNumberAsync(long tariffId, string phoneNumber);
    Task CreateAsync(Rate rate);
    Task UpdateAsync(Rate rate);
    Task DeleteAsync(Rate rate);
}
