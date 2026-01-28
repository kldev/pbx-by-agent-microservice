using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Data.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("users");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Gid)
            .HasColumnName("gid")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Department)
            .HasColumnName("department")
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.StructureId)
            .HasColumnName("structure_id")
            .IsRequired();

        builder.Property(e => e.TeamId)
            .HasColumnName("team_id");

        builder.Property(e => e.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(e => e.LastLoginAt)
            .HasColumnName("last_login_at");

        builder.Property(e => e.SupervisorId)
            .HasColumnName("supervisor_id");

        // BaseAuditableTable properties
        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.CreatedByUserId)
            .HasColumnName("created_by_user_id");

        builder.Property(e => e.ModifiedAt)
            .HasColumnName("modified_at");

        builder.Property(e => e.ModifiedByUserId)
            .HasColumnName("modified_by_user_id");

        builder.Property(e => e.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(e => e.DeletedAt)
            .HasColumnName("deleted_at");

        // Relationships
        builder.HasOne(e => e.Structure)
            .WithMany(s => s.Employees)
            .HasForeignKey(e => e.StructureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Team)
            .WithMany(t => t.Employees)
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        // Self-referencing: Supervisor-Subordinates relationship
        builder.HasOne(e => e.Supervisor)
            .WithMany(e => e.Subordinates)
            .HasForeignKey(e => e.SupervisorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.Gid)
            .IsUnique();

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.HasIndex(e => e.StructureId);

        builder.HasIndex(e => e.TeamId);

        builder.HasIndex(e => e.SupervisorId);
    }
}
