using AnswerRule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AnswerRule.Data.Configurations;

public class AnsweringRuleTimeSlotConfiguration : IEntityTypeConfiguration<AnsweringRuleTimeSlot>
{
    public void Configure(EntityTypeBuilder<AnsweringRuleTimeSlot> builder)
    {
        builder.ToTable("answering_rule_time_slots");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Gid).HasColumnName("gid").HasMaxLength(32);

        builder.Property(x => x.AnsweringRuleId).HasColumnName("answering_rule_id");
        builder.Property(x => x.DayOfWeek).HasColumnName("day_of_week");
        builder.Property(x => x.StartTime).HasColumnName("start_time");
        builder.Property(x => x.EndTime).HasColumnName("end_time");
        builder.Property(x => x.IsAllDay).HasColumnName("is_all_day");

        // Indexes
        builder.HasIndex(x => x.Gid).IsUnique();
        builder.HasIndex(x => x.AnsweringRuleId);
        
        var timeOnlyConverter = new ValueConverter<TimeOnly, TimeSpan>(
            v => v.ToTimeSpan(),
            v => TimeOnly.FromTimeSpan(v)
        );

        builder.Property(x => x.StartTime)
            .HasConversion(timeOnlyConverter);
        
        builder.Property(x => x.EndTime)
            .HasConversion(timeOnlyConverter);
        
        builder.Property(x=>x.DayOfWeek).HasConversion( v => v.ToString(), v => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), v));
        
        
    }
}
