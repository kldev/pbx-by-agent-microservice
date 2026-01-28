using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Data.Configurations;

public class LoginAuditLogConfiguration : IEntityTypeConfiguration<LoginAuditLog>
{
    public void Configure(EntityTypeBuilder<LoginAuditLog> builder)
    {
        builder.ToTable("login_audit_logs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.AppUserId)
            .HasColumnName("app_user_id")
            .IsRequired();

        builder.Property(e => e.LoginAt)
            .HasColumnName("login_at")
            .IsRequired();

        builder.Property(e => e.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45);

        builder.Property(e => e.UserAgent)
            .HasColumnName("user_agent")
            .HasMaxLength(500);

        builder.Property(e => e.Success)
            .HasColumnName("success")
            .IsRequired();
        
        builder.Property(e => e.UserEmail)
            .HasColumnName("user_email")
            .HasMaxLength(100);
        
        builder.Property(e => e.UserFullname)
            .HasColumnName("user_fullname")
            .HasMaxLength(255);

        builder.HasOne(e => e.AppUser)
            .WithMany(emp => emp.LoginAuditLogs)
            .HasForeignKey(e => e.AppUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(e => e.AppUserId);

        builder.HasIndex(e => e.LoginAt);
    }
}
