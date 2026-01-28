using RateService.Api.Features.Tariffs.Model;
using RateService.Data.Entities;

namespace RateService.Api.Features.Tariffs;

public interface ITariffDataHandler
{
    Task<(IEnumerable<Tariff> Items, int TotalCount)> GetPagedAsync(TariffListFilter filter);
    Task<Tariff?> GetByGidAsync(string gid);
    Task<Tariff?> GetByGidWithRatesAsync(string gid);
    Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
    Task<Tariff?> GetDefaultTariffAsync();
    Task CreateAsync(Tariff tariff);
    Task UpdateAsync(Tariff tariff);
    Task DeleteAsync(Tariff tariff);
    Task ClearDefaultFlagAsync(long? excludeId = null);
}
