using AnswerRule.Data;
using AnswerRule.Data.Entities;
using AnswerRule.Data.Enums;

namespace AnswerRule.Api.Tests.Infrastructure;

public static class AnswerRuleTestDataSeeder
{
    public static readonly string TestSipAccountGid = "testsip00100010001000100010001";
    public static readonly string TestRuleGid = "testrule0010001000100010001001";
    public static readonly string TestVoicemailBoxGid = "testvm00100010001000100010001";

    public static void SeedTestData(AnswerRuleDbContext context)
    {
        var rule = new AnsweringRule
        {
            Gid = TestRuleGid,
            SipAccountGid = TestSipAccountGid,
            Name = "Test Rule",
            Description = "Test rule description",
            Priority = 100,
            IsEnabled = true,
            ActionType = AnsweringRuleAction.Voicemail,
            VoicemailBoxGid = TestVoicemailBoxGid,
            SendEmailNotification = false,
            TimeSlots = new List<AnsweringRuleTimeSlot>
            {
                new()
                {
                    Gid = Guid.NewGuid().ToString("N"),
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeOnly(17, 0),
                    EndTime = new TimeOnly(23, 0),
                    IsAllDay = false
                },
                new()
                {
                    Gid = Guid.NewGuid().ToString("N"),
                    DayOfWeek = DayOfWeek.Saturday,
                    StartTime = new TimeOnly(0, 0),
                    EndTime = new TimeOnly(23, 59),
                    IsAllDay = true
                }
            }
        };

        context.AnsweringRules.Add(rule);
    }
}
