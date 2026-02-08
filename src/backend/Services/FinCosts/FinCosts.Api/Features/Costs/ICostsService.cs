using App.Shared.Web.BaseModel;
using FinCosts.Api.Features.Costs.Model;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;

namespace FinCosts.Api.Features.Costs;

public interface ICostsService
{
    public Task<Result<List<DocumentTypeResponse>>> GetDocumentTypes(PortalAuthInfo auth);
    public Task<Result<List<CurrencyTypeResponse>>> GetCurrencyTypes(PortalAuthInfo auth);
    public Task<Result<List<VatRateTypeResponse>>> GetVatRateTypes(PortalAuthInfo auth);
    public Task<Result<PagedResult<DocumentEntryResponse>>> GetDocumentEntries(PortalAuthInfo auth, DocumentEntryListFilter filter);
}