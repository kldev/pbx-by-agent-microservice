using AnswerRule.Data.Entities;
using AnswerRule.Data.Enums;

namespace AnswerRule.Api.Seed;

public static class ShowcaseData
{
    public static List<AnsweringRule> GetRules() =>
    [
        new AnsweringRule
        {
            Gid = FixedGuids.AfterHoursRule,
            SipAccountGid = FixedGuids.TestSipAccount1,
            Name = "Poza godzinami pracy",
            Description = "Przekierowanie na voicemail po godzinach pracy i w weekendy",
            Priority = 100,
            IsEnabled = true,
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = FixedGuids.TestVoicemailBox,
            SendEmailNotification = true,
            TimeSlots = GetAfterHoursTimeSlots()
        },
        new AnsweringRule
        {
            Gid = FixedGuids.LunchBreakRule,
            SipAccountGid = FixedGuids.TestSipAccount1,
            Name = "Przerwa obiadowa",
            Description = "Przekierowanie na grupę wsparcia podczas przerwy obiadowej",
            Priority = 50,
            IsEnabled = true,
            ActionType = AnsweringRuleAction.RedirectToGroup,
            ActionTarget = FixedGuids.TestSupportGroup,
            SendEmailNotification = false,
            TimeSlots = GetLunchBreakTimeSlots()
        }
    ];

    private static List<AnsweringRuleTimeSlot> GetAfterHoursTimeSlots()
    {
        var slots = new List<AnsweringRuleTimeSlot>();

        // Weekdays evening (17:00 - 24:00)
        for (int day = 1; day <= 5; day++)
        {
            slots.Add(new AnsweringRuleTimeSlot
            {
                Gid = Guid.NewGuid().ToString("N"),
                DayOfWeek = (DayOfWeek)day,
                StartTime = new TimeOnly(17, 0),
                EndTime = new TimeOnly(23, 59),
                IsAllDay = false
            });
        }

        // Weekdays morning (00:00 - 08:00)
        for (int day = 1; day <= 5; day++)
        {
            slots.Add(new AnsweringRuleTimeSlot
            {
                Gid = Guid.NewGuid().ToString("N"),
                DayOfWeek = (DayOfWeek)day,
                StartTime = new TimeOnly(0, 0),
                EndTime = new TimeOnly(8, 0),
                IsAllDay = false
            });
        }

        // Weekend all day
        slots.Add(new AnsweringRuleTimeSlot
        {
            Gid = Guid.NewGuid().ToString("N"),
            DayOfWeek = DayOfWeek.Saturday,
            StartTime = new TimeOnly(0, 0),
            EndTime = new TimeOnly(23, 59),
            IsAllDay = true
        });
        slots.Add(new AnsweringRuleTimeSlot
        {
            Gid = Guid.NewGuid().ToString("N"),
            DayOfWeek = DayOfWeek.Sunday,
            StartTime = new TimeOnly(0, 0),
            EndTime = new TimeOnly(23, 59),
            IsAllDay = true
        });

        return slots;
    }

    private static List<AnsweringRuleTimeSlot> GetLunchBreakTimeSlots()
    {
        var slots = new List<AnsweringRuleTimeSlot>();

        // Weekdays lunch (12:00 - 13:00)
        for (int day = 1; day <= 5; day++)
        {
            slots.Add(new AnsweringRuleTimeSlot
            {
                Gid = Guid.NewGuid().ToString("N"),
                DayOfWeek = (DayOfWeek)day,
                StartTime = new TimeOnly(12, 0),
                EndTime = new TimeOnly(13, 0),
                IsAllDay = false
            });
        }

        return slots;
    }
}
