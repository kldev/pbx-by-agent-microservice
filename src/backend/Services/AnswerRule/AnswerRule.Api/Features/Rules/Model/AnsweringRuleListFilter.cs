using AnswerRule.Data.Enums;
using App.Shared.Web.BaseModel;

namespace AnswerRule.Api.Features.Rules.Model;

public class AnsweringRuleListFilter : PagedRequest
{
    public string? SipAccountGid { get; set; }
    public string? Search { get; set; }
    public bool? IsEnabled { get; set; }
    public AnsweringRuleAction? ActionType { get; set; }
}
