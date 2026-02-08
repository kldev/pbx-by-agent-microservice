using App.BaseData;

namespace FinCosts.Data.Entities;

public class VatRateTypeDict : BaseTranslatedDict
{
    public decimal VatRate { get; set; } = 0M;
}