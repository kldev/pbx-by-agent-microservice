using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using FinCosts.Api.Features.Costs.Model;
using FinCosts.Data;
using Microsoft.EntityFrameworkCore;

namespace FinCosts.Api.Features.Costs;

public class CostsService : ICostsService
{
    private readonly FinCostsDbContext _context;

    // ReSharper disable once ConvertToPrimaryConstructor
    public CostsService(FinCostsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<DocumentTypeResponse>>> GetDocumentTypes(PortalAuthInfo auth)
    {
        var data = _context.CostDocumentTypeReadOnly
            .Select(z => new DocumentTypeResponse()
        {
            Id = z.Id, NameEN = z.NameEN ?? "", NamePL = z.NamePL
        });

        return Result<List<DocumentTypeResponse>>.Success(await data.ToListAsync());
    }

    public async Task<Result<List<CurrencyTypeResponse>>> GetCurrencyTypes(PortalAuthInfo auth)
    {
        var data = _context.CurrencyTypeReadOnly
            .Select(z => new CurrencyTypeResponse
            {
                Id = z.Id, NameEN = z.NameEN ?? "", NamePL = z.NamePL, Category = z.Category ?? ""
            });

        return Result<List<CurrencyTypeResponse>>.Success(await data.ToListAsync());
    }

    public async Task<Result<List<VatRateTypeResponse>>> GetVatRateTypes(PortalAuthInfo auth)
    {
        var data = _context.VatRateTypeReadOnly
            .Select(z => new VatRateTypeResponse
            {
                Id = z.Id, NameEN = z.NameEN ?? "", NamePL = z.NamePL, VatRate = z.VatRate
            });

        return Result<List<VatRateTypeResponse>>.Success(await data.ToListAsync());
    }

    public async Task<Result<PagedResult<DocumentEntryResponse>>> GetDocumentEntries(PortalAuthInfo auth, DocumentEntryListFilter filter)
    {
        var query = _context.DocumentEntryReadOnly
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(x =>
                x.IssuedBy.Contains(search) ||
                x.IssuedFor.Contains(search) ||
                (x.KSERefNumber != null && x.KSERefNumber.Contains(search)));
        }

        if (filter.DocumentTypeId.HasValue)
            query = query.Where(x => x.DocumentTypeId == filter.DocumentTypeId.Value);

        if (!string.IsNullOrWhiteSpace(filter.CurrencyNamePL))
            query = query.Where(x => x.CurrencyNamePL == filter.CurrencyNamePL);

        if (filter.WasPaid.HasValue)
            query = query.Where(x => x.WasPaid == filter.WasPaid.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .Select(x => new DocumentEntryResponse
            {
                Gid = x.Gid,
                CurrencyNamePL = x.CurrencyNamePL,
                CurrencyForeignRate = x.CurrencyForeignRate,
                TotalAmount = x.TotalAmount,
                VatRate = x.VatRate,
                WasPaid = x.WasPaid,
                KSERefNumber = x.KSERefNumber,
                KSEAccountNumber = x.KSEAccountNumber,
                DocumentTypeId = x.DocumentTypeId,
                DocumentTypeNamePL = x.DocumentTypeNamePL,
                IssuedBy = x.IssuedBy,
                IssuedVatNumber = x.IssuedVatNumber,
                IssuedFor = x.IssuedFor,
                IssuedForVatNumber = x.IssuedForVatNumber,
                CreatedAt = x.CreatedAt,
                ModifiedAt = x.ModifiedAt
            })
            .ToListAsync();

        var result = new PagedResult<DocumentEntryResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

        return Result<PagedResult<DocumentEntryResponse>>.Success(result);
    }
}