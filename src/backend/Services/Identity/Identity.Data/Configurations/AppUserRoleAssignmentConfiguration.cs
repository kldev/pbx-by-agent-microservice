using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Data.Configurations;

public class AppUserRoleAssignmentConfiguration : IEntityTypeConfiguration<AppUserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<AppUserRoleAssignment> builder)
    {
        builder.ToTable("app_user_role_assignments");

        // Composite primary key
        builder.HasKey(e => new { e.UserId, e.RoleId });

        builder.Property(e => e.UserId)
            .HasColumnName("user_id");

        builder.Property(e => e.RoleId)
            .HasColumnName("role_id");

        builder.Property(e => e.AssignedAt)
            .HasColumnName("assigned_at")
            .IsRequired();

        builder.Property(e => e.AssignedBy)
            .HasColumnName("assigned_by");

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany(u => u.RoleAssignments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Role)
            .WithMany(r => r.UserAssignments)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.RoleId);
    }
}
