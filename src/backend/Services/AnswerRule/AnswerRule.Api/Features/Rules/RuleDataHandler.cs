using AnswerRule.Api.Features.Rules.Model;
using AnswerRule.Data;
using AnswerRule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnswerRule.Api.Features.Rules;

public class RuleDataHandler : IRuleDataHandler
{
    private readonly AnswerRuleDbContext _context;

    public RuleDataHandler(AnswerRuleDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<AnsweringRule> Items, int TotalCount)> GetPagedAsync(AnsweringRuleListFilter filter)
    {
        var query = _context.AnsweringRules
            .Include(r => r.TimeSlots)
            .Where(r => !r.IsDeleted)
            .AsQueryable();

        // Filter by SIP account
        if (!string.IsNullOrWhiteSpace(filter.SipAccountGid))
        {
            query = query.Where(r => r.SipAccountGid == filter.SipAccountGid);
        }

        // Search
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(r =>
                r.Name.ToLower().Contains(search) ||
                (r.Description != null && r.Description.ToLower().Contains(search)));
        }

        // Filter by enabled
        if (filter.IsEnabled.HasValue)
        {
            query = query.Where(r => r.IsEnabled == filter.IsEnabled);
        }

        // Filter by action type
        if (filter.ActionType.HasValue)
        {
            query = query.Where(r => r.ActionType == filter.ActionType);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(r => r.Priority)
            .ThenByDescending(r => r.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<AnsweringRule?> GetByGidAsync(string gid)
    {
        return await _context.AnsweringRules
            .FirstOrDefaultAsync(r => r.Gid == gid && !r.IsDeleted);
    }

    public async Task<AnsweringRule?> GetByGidWithTimeSlotsAsync(string gid)
    {
        return await _context.AnsweringRules
            .Include(r => r.TimeSlots)
            .FirstOrDefaultAsync(r => r.Gid == gid && !r.IsDeleted);
    }

    public async Task<List<AnsweringRule>> GetActiveRulesForSipAccountAsync(string sipAccountGid)
    {
        return await _context.AnsweringRules
            .Include(r => r.TimeSlots)
            .Where(r => r.SipAccountGid == sipAccountGid && r.IsEnabled && !r.IsDeleted)
            .OrderBy(r => r.Priority)
            .ThenByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task CreateAsync(AnsweringRule rule)
    {
        _context.AnsweringRules.Add(rule);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AnsweringRule rule)
    {
        _context.AnsweringRules.Update(rule);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(AnsweringRule rule)
    {
        rule.IsDeleted = true;
        rule.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTimeSlotsAsync(long ruleId)
    {
        var slots = await _context.AnsweringRuleTimeSlots
            .Where(s => s.AnsweringRuleId == ruleId)
            .ToListAsync();

        _context.AnsweringRuleTimeSlots.RemoveRange(slots);
        await _context.SaveChangesAsync();
    }

    public async Task AddTimeSlotsAsync(IEnumerable<AnsweringRuleTimeSlot> timeSlots)
    {
        _context.AnsweringRuleTimeSlots.AddRange(timeSlots);
        await _context.SaveChangesAsync();
    }
}
