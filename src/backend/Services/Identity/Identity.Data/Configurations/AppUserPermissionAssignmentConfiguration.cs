using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Data.Configurations;

public class AppUserPermissionAssignmentConfiguration : IEntityTypeConfiguration<AppUserPermissionAssignment>
{
    public void Configure(EntityTypeBuilder<AppUserPermissionAssignment> builder)
    {
        builder.ToTable("app_user_permission_assignments");

        // Composite primary key
        builder.HasKey(e => new { e.UserId, e.PermissionId });

        builder.Property(e => e.UserId)
            .HasColumnName("user_id");

        builder.Property(e => e.PermissionId)
            .HasColumnName("permission_id");

        builder.Property(e => e.GrantedAt)
            .HasColumnName("granted_at")
            .IsRequired();

        builder.Property(e => e.GrantedBy)
            .HasColumnName("granted_by");

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany(u => u.PermissionAssignments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Permission)
            .WithMany(p => p.UserAssignments)
            .HasForeignKey(e => e.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.PermissionId);
    }
}
