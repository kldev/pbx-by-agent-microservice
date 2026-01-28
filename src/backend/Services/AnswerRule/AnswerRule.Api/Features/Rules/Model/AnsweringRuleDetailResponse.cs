namespace AnswerRule.Api.Features.Rules.Model;

public class AnsweringRuleDetailResponse : AnsweringRuleResponse
{
    public string SipAccountGid { get; set; } = null!;
    public string? VoicemailBoxGid { get; set; }
    public string? VoiceMessageGid { get; set; }
    public string? NotificationEmail { get; set; }
    public List<TimeSlotDto> TimeSlots { get; set; } = [];
}
