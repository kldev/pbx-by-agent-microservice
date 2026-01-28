using AnswerRule.Data.Enums;

namespace AnswerRule.Api.Features.Rules.Model;

public class CreateAnsweringRuleRequest
{
    public string SipAccountGid { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Priority { get; set; } = 100;
    public bool IsEnabled { get; set; } = true;
    public AnsweringRuleAction ActionType { get; set; }
    public string? ActionTarget { get; set; }
    public string? VoicemailBoxGid { get; set; }
    public string? VoiceMessageGid { get; set; }
    public bool SendEmailNotification { get; set; }
    public string? NotificationEmail { get; set; }
    public List<TimeSlotDto> TimeSlots { get; set; } = [];
}
