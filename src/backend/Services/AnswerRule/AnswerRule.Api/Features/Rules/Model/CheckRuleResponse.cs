using AnswerRule.Data.Enums;

namespace AnswerRule.Api.Features.Rules.Model;

public class CheckRuleResponse
{
    public bool HasActiveRule { get; set; }
    public ActiveRuleInfo? Rule { get; set; }
}

public class ActiveRuleInfo
{
    public string Gid { get; set; } = null!;
    public string Name { get; set; } = null!;
    public AnsweringRuleAction ActionType { get; set; }
    public string? ActionTarget { get; set; }
    public string? VoicemailBoxGid { get; set; }
    public string? VoiceMessageGid { get; set; }
    public bool SendEmailNotification { get; set; }
    public string? NotificationEmail { get; set; }
}
