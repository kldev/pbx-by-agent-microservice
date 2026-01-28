namespace AnswerRule.Api.Features.Rules.Model;

public class CheckRuleRequest
{
    public string SipAccountGid { get; set; } = null!;
    public DateTime? CheckDateTime { get; set; }
}
