using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Data.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("password_reset_tokens");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.AppUserId)
            .HasColumnName("app_user_id")
            .IsRequired();

        builder.Property(e => e.Token)
            .HasColumnName("token")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(e => e.IsUsed)
            .HasColumnName("is_used")
            .HasDefaultValue(false);

        builder.Property(e => e.UsedAt)
            .HasColumnName("used_at");

        // Relationships
        builder.HasOne(e => e.AppUser)
            .WithMany(u => u.PasswordResetTokens)
            .HasForeignKey(e => e.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.Token)
            .IsUnique();

        builder.HasIndex(e => e.AppUserId);

        builder.HasIndex(e => e.ExpiresAt);
    }
}
