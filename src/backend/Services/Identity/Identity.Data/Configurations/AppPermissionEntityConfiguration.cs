using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Data.Configurations;

public class AppPermissionEntityConfiguration : IEntityTypeConfiguration<AppPermissionEntity>
{
    public void Configure(EntityTypeBuilder<AppPermissionEntity> builder)
    {
        builder.ToTable("app_permissions");

        builder.HasKey(e => e.Id);

        // No auto-increment - ID maps to AppPermission enum
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(e => e.Category)
            .HasColumnName("category")
            .HasMaxLength(100);

        builder.Property(e => e.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(e => e.IsSpecial)
            .HasColumnName("is_special")
            .HasDefaultValue(false);

        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.HasIndex(e => e.Category);
    }
}
