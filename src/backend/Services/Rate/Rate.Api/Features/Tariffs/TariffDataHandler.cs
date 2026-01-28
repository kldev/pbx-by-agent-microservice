using Microsoft.EntityFrameworkCore;
using RateService.Api.Features.Tariffs.Model;
using RateService.Data;
using RateService.Data.Entities;

namespace RateService.Api.Features.Tariffs;

public class TariffDataHandler : ITariffDataHandler
{
    private readonly RateServiceDbContext _context;

    public TariffDataHandler(RateServiceDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Tariff> Items, int TotalCount)> GetPagedAsync(TariffListFilter filter)
    {
        var query = _context.Tariffs
            .Where(t => !t.IsDeleted)
            .AsQueryable();

        // Filtrowanie
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(search) ||
                (t.Description != null && t.Description.ToLower().Contains(search)));
        }

        if (filter.IsActive.HasValue)
            query = query.Where(t => t.IsActive == filter.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.CurrencyCode))
            query = query.Where(t => t.CurrencyCode == filter.CurrencyCode);

        // Liczenie
        var totalCount = await query.CountAsync();

        // Paginacja
        var items = await query
            .OrderByDescending(t => t.IsDefault)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => new Tariff
            {
                Id = t.Id,
                Gid = t.Gid,
                Name = t.Name,
                Description = t.Description,
                CurrencyCode = t.CurrencyCode,
                IsDefault = t.IsDefault,
                IsActive = t.IsActive,
                ValidFrom = t.ValidFrom,
                ValidTo = t.ValidTo,
                BillingIncrement = t.BillingIncrement,
                MinimumDuration = t.MinimumDuration,
                ConnectionFee = t.ConnectionFee,
                CreatedAt = t.CreatedAt,
                Rates = t.Rates.Where(r => !r.IsDeleted).ToList()
            })
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Tariff?> GetByGidAsync(string gid)
    {
        return await _context.Tariffs
            .FirstOrDefaultAsync(t => t.Gid == gid && !t.IsDeleted);
    }

    public async Task<Tariff?> GetByGidWithRatesAsync(string gid)
    {
        return await _context.Tariffs
            .Include(t => t.Rates.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.DestinationGroup)
            .FirstOrDefaultAsync(t => t.Gid == gid && !t.IsDeleted);
    }

    public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
    {
        var query = _context.Tariffs.Where(t => t.Name == name && !t.IsDeleted);
        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId);
        return await query.AnyAsync();
    }

    public async Task<Tariff?> GetDefaultTariffAsync()
    {
        return await _context.Tariffs
            .FirstOrDefaultAsync(t => t.IsDefault && !t.IsDeleted);
    }

    public async Task CreateAsync(Tariff tariff)
    {
        _context.Tariffs.Add(tariff);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Tariff tariff)
    {
        _context.Tariffs.Update(tariff);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tariff tariff)
    {
        tariff.IsDeleted = true;
        tariff.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task ClearDefaultFlagAsync(long? excludeId = null)
    {
        var query = _context.Tariffs.Where(t => t.IsDefault && !t.IsDeleted);
        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId);

        var tariffs = await query.ToListAsync();
        foreach (var tariff in tariffs)
        {
            tariff.IsDefault = false;
        }
        await _context.SaveChangesAsync();
    }
}
