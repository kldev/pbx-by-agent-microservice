namespace AnswerRule.Api.Features.Rules.Model;

public class TimeSlotDto
{
    public string? Gid { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string StartTime { get; set; } = null!;
    public string EndTime { get; set; } = null!;
    public bool IsAllDay { get; set; }
}
