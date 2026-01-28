using AnswerRule.Api.Features.Rules.Model;
using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;

namespace AnswerRule.Api.Features.Rules;

public interface IRuleService
{
    Task<Result<PagedResult<AnsweringRuleResponse>>> GetListAsync(PortalAuthInfo auth, AnsweringRuleListFilter filter);
    Task<Result<AnsweringRuleDetailResponse>> GetByGidAsync(PortalAuthInfo auth, string gid);
    Task<Result<AnsweringRuleDetailResponse>> CreateAsync(PortalAuthInfo auth, CreateAnsweringRuleRequest request);
    Task<Result<AnsweringRuleDetailResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateAnsweringRuleRequest request);
    Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid);
    Task<Result<AnsweringRuleDetailResponse>> ToggleAsync(PortalAuthInfo auth, string gid);
    Task<Result<CheckRuleResponse>> CheckActiveRuleAsync(PortalAuthInfo auth, CheckRuleRequest request);
}
