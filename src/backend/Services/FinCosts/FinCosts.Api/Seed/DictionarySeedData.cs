using App.Bps.Enum.FinCosts;
using FinCosts.Data;
using FinCosts.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinCosts.Api.Seed;

public static class DictionarySeedData
{
    public static async Task SeedAsync(FinCostsDbContext context)
    {
        var documentTypes = GetDocumentTypes();

        var hasDocumentsSeeded = await context.CostDocumentTypeReadOnly.AnyAsync();
        
        if (!hasDocumentsSeeded)
        {
            await context.CostDocumentTypeSeed.AddRangeAsync(documentTypes);
            await context.SaveChangesAsync();
        }
        
        var currencyTypes = GetCurrencyTypes();
        var hasCurrencyTypesSeed = await context.CurrencyTypeReadOnly.AnyAsync();

        if (!hasCurrencyTypesSeed)
        {
            await context.CurrencyTypeSeed.AddRangeAsync(currencyTypes);
            await context.SaveChangesAsync();
        }

        var vatRateTypes = GetVatRateTypes();
        var hasVatRateTypesSeed = await context.VatRateTypeReadOnly.AnyAsync();

        if (!hasVatRateTypesSeed)
        {
            await context.VatRateTypeSeed.AddRangeAsync(vatRateTypes);
            await context.SaveChangesAsync();
        }
    }
    
    private static List<CostDocumentTypeDict> GetDocumentTypes()
    {
        return
        [
            new ()
            {
                Id = 1, NamePL = "Faktura", NameEN = nameof(CostDocumentType.Invoice), Category = "Invoices",
                IsActive = true
            },

            new()
            {
                Id=2, NamePL = "Pro forma", NameEN = nameof(CostDocumentType.ProForma), Category = "Invoices", IsActive = true
            },
            new ()
            {
                Id = 99, NamePL = "Inne", NameEN =  nameof(CostDocumentType.Other), Category = "Others", IsActive = true
            }
        ];
    }

    private static List<CurrencyTypeDict> GetCurrencyTypes()
    {
        return
        [
            new CurrencyTypeDict()
            {
                Id = 1, IsActive = true, NamePL = nameof(CostCurrencyType.PLN), NameEN = nameof(CostCurrencyType.PLN), Category = "Poland"
            },
            new CurrencyTypeDict()
            {
                Id = 2, IsActive = true, NamePL = nameof(CostCurrencyType.EUR), NameEN = nameof(CostCurrencyType.EUR), Category = "Foreign"
            },
            new CurrencyTypeDict()
            {
                Id = 3, IsActive = true, NamePL = nameof(CostCurrencyType.USD), NameEN = nameof(CostCurrencyType.USD), Category = "Foreign"
            },
            new CurrencyTypeDict()
            {
                Id = 4, IsActive = true, NamePL = nameof(CostCurrencyType.DKK), NameEN = nameof(CostCurrencyType.DKK), Category = "Foreign"
            }
        ];
    }

    private static List<VatRateTypeDict> GetVatRateTypes()
    {
        return
        [
            new VatRateTypeDict { Id = 1, IsActive = true, NamePL = "0%", NameEN = "0%", VatRate = 0M },
            new VatRateTypeDict { Id = 2, IsActive = true, NamePL = "7%", NameEN = "7%", VatRate = 7M },
            new VatRateTypeDict { Id = 3, IsActive = true, NamePL = "9%", NameEN = "9%", VatRate = 9M },
            new VatRateTypeDict { Id = 4, IsActive = true, NamePL = "13%", NameEN = "13%", VatRate = 13M },
            new VatRateTypeDict { Id = 5, IsActive = true, NamePL = "21%", NameEN = "21%", VatRate = 21M },
            new VatRateTypeDict { Id = 6, IsActive = true, NamePL = "23%", NameEN = "23%", VatRate = 23M }
        ];
    }
}