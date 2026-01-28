using AnswerRule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnswerRule.Data;

public class AnswerRuleDbContext : DbContext
{
    public DbSet<AnsweringRule> AnsweringRules => Set<AnsweringRule>();
    public DbSet<AnsweringRuleTimeSlot> AnsweringRuleTimeSlots => Set<AnsweringRuleTimeSlot>();

    public AnswerRuleDbContext(DbContextOptions<AnswerRuleDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnswerRuleDbContext).Assembly);
    }
}
