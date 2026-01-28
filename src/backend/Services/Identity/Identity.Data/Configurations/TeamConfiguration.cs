using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("teams");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Gid)
            .HasColumnName("gid")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Code)
            .HasColumnName("code")
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.StructureId)
            .HasColumnName("structure_id")
            .IsRequired();

        builder.Property(e => e.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

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
            .WithMany(s => s.Teams)
            .HasForeignKey(e => e.StructureId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.Gid)
            .IsUnique();

        builder.HasIndex(e => e.Code)
            .IsUnique();

        builder.HasIndex(e => new { e.StructureId, e.Name })
            .IsUnique();
    }
}
