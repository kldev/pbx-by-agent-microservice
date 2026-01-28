using System.Text.Json.Serialization;
using AnswerRule.Data.Enums;
using App.BaseData;

namespace AnswerRule.Data.Entities;

public class AnsweringRule : BaseAuditableTable
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

    // Navigation
    public ICollection<AnsweringRuleTimeSlot> TimeSlots { get; set; } = new List<AnsweringRuleTimeSlot>();
}
