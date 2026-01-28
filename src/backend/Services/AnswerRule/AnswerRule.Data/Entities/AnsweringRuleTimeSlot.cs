using System.Text.Json.Serialization;
using App.BaseData;

namespace AnswerRule.Data.Entities;

public class AnsweringRuleTimeSlot : BaseTable
{
    public long AnsweringRuleId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAllDay { get; set; }

    // Navigation
    public AnsweringRule AnsweringRule { get; set; } = null!;
}
