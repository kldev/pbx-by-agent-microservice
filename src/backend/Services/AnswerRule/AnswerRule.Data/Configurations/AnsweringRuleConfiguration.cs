using AnswerRule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnswerRule.Data.Configurations;

public class AnsweringRuleConfiguration : IEntityTypeConfiguration<AnsweringRule>
{
    public void Configure(EntityTypeBuilder<AnsweringRule> builder)
    {
        builder.ToTable("answering_rules");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Gid).HasColumnName("gid").HasMaxLength(32);

        builder.Property(x => x.SipAccountGid).HasColumnName("sip_account_gid").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(x => x.Priority).HasColumnName("priority");
        builder.Property(x => x.IsEnabled).HasColumnName("is_enabled");
        builder.Property(x => x.ActionType).HasColumnName("action_type");
        builder.Property(x => x.ActionTarget).HasColumnName("action_target").HasMaxLength(255);
        builder.Property(x => x.VoicemailBoxGid).HasColumnName("voicemail_box_gid").HasMaxLength(32);
        builder.Property(x => x.VoiceMessageGid).HasColumnName("voice_message_gid").HasMaxLength(32);
        builder.Property(x => x.SendEmailNotification).HasColumnName("send_email_notification");
        builder.Property(x => x.NotificationEmail).HasColumnName("notification_email").HasMaxLength(255);

        // Audit fields
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(x => x.ModifiedAt).HasColumnName("modified_at");
        builder.Property(x => x.ModifiedByUserId).HasColumnName("modified_by_user_id");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        // Indexes
        builder.HasIndex(x => x.Gid).IsUnique();
        builder.HasIndex(x => x.SipAccountGid);
        builder.HasIndex(x => new { x.IsEnabled, x.Priority });

        // Relationship
        builder.HasMany(x => x.TimeSlots)
            .WithOne(x => x.AnsweringRule)
            .HasForeignKey(x => x.AnsweringRuleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.ActionType)
            .HasColumnName("action_type")
            .IsRequired()
            .HasConversion<string>();
    }
}
