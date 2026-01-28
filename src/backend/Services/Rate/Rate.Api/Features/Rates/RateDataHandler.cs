using Microsoft.EntityFrameworkCore;
using RateService.Api.Features.Rates.Model;
using RateService.Data;
using RateService.Data.Entities;

namespace RateService.Api.Features.Rates;

public class RateDataHandler : IRateDataHandler
{
    private readonly RateServiceDbContext _context;

    public RateDataHandler(RateServiceDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Rate> Items, int TotalCount)> GetPagedAsync(RateListFilter filter)
    {
        var query = _context.Rates
            .Include(r => r.Tariff)
            .Include(r => r.DestinationGroup)
            .Where(r => !r.IsDeleted)
            .AsQueryable();

        // Filtrowanie
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(r =>
                r.Prefix.ToLower().Contains(search) ||
                r.DestinationName.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filter.TariffGid))
            query = query.Where(r => r.Tariff!.Gid == filter.TariffGid);

        if (!string.IsNullOrWhiteSpace(filter.Prefix))
            query = query.Where(r => r.Prefix.StartsWith(filter.Prefix));

        if (filter.DestinationGroupId.HasValue)
            query = query.Where(r => r.DestinationGroupId == filter.DestinationGroupId);

        if (filter.IsActive.HasValue)
            query = query.Where(r => r.IsActive == filter.IsActive);

        // Liczenie
        var totalCount = await query.CountAsync();

        // Paginacja - sortowanie po prefiksie
        var items = await query
            .OrderBy(r => r.Prefix)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Rate?> GetByGidAsync(string gid)
    {
        return await _context.Rates
            .Include(r => r.Tariff)
            .Include(r => r.DestinationGroup)
            .FirstOrDefaultAsync(r => r.Gid == gid && !r.IsDeleted);
    }

    public async Task<bool> ExistsByPrefixInTariffAsync(long tariffId, string prefix, long? excludeId = null)
    {
        var query = _context.Rates
            .Where(r => r.TariffId == tariffId && r.Prefix == prefix && !r.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(r => r.Id != excludeId);

        return await query.AnyAsync();
    }

    /// <summary>
    /// Znajduje stawkę dla numeru telefonu używając Longest Prefix Match
    /// </summary>
    public async Task<Rate?> FindRateForNumberAsync(long tariffId, string phoneNumber)
    {
        // Normalizuj numer (usuń spacje, myślniki)
        var normalizedNumber = phoneNumber.Replace(" ", "").Replace("-", "");

        // Pobierz wszystkie aktywne stawki dla taryfy
        var rates = await _context.Rates
            .Include(r => r.Tariff)
            .Where(r => r.TariffId == tariffId &&
                        r.IsActive &&
                        !r.IsDeleted &&
                        r.EffectiveFrom <= DateTime.UtcNow &&
                        (r.EffectiveTo == null || r.EffectiveTo >= DateTime.UtcNow))
            .ToListAsync();

        // Longest Prefix Match - znajdź najdłuższy pasujący prefiks
        Rate? bestMatch = null;
        int longestMatchLength = 0;

        foreach (var rate in rates)
        {
            if (normalizedNumber.StartsWith(rate.Prefix) && rate.Prefix.Length > longestMatchLength)
            {
                bestMatch = rate;
                longestMatchLength = rate.Prefix.Length;
            }
        }

        return bestMatch;
    }

    public async Task CreateAsync(Rate rate)
    {
        _context.Rates.Add(rate);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Rate rate)
    {
        _context.Rates.Update(rate);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Rate rate)
    {
        rate.IsDeleted = true;
        rate.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
