using AnswerRule.Data.Enums;

namespace AnswerRule.Api.Features.Rules.Model;

public class AnsweringRuleResponse
{
    public string Gid { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public bool IsEnabled { get; set; }
    public AnsweringRuleAction ActionType { get; set; }
    public string? ActionTarget { get; set; }
    public bool SendEmailNotification { get; set; }
    public int TimeSlotsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
