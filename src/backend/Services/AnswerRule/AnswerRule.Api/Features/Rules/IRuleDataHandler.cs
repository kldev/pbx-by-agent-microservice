using AnswerRule.Api.Features.Rules.Model;
using AnswerRule.Data.Entities;

namespace AnswerRule.Api.Features.Rules;

public interface IRuleDataHandler
{
    Task<(IEnumerable<AnsweringRule> Items, int TotalCount)> GetPagedAsync(AnsweringRuleListFilter filter);
    Task<AnsweringRule?> GetByGidAsync(string gid);
    Task<AnsweringRule?> GetByGidWithTimeSlotsAsync(string gid);
    Task<List<AnsweringRule>> GetActiveRulesForSipAccountAsync(string sipAccountGid);
    Task CreateAsync(AnsweringRule rule);
    Task UpdateAsync(AnsweringRule rule);
    Task DeleteAsync(AnsweringRule rule);
    Task DeleteTimeSlotsAsync(long ruleId);
    Task AddTimeSlotsAsync(IEnumerable<AnsweringRuleTimeSlot> timeSlots);
}
