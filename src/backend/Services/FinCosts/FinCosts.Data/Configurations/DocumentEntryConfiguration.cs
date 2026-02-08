using FinCosts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinCosts.Data.Configurations;

public class DocumentEntryConfiguration : IEntityTypeConfiguration<DocumentEntry>
{
    public void Configure(EntityTypeBuilder<DocumentEntry> builder)
    {
        builder.ToTable("document_entry");

        builder.HasKey(x => x.Id);

        // BaseTable
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.Gid).HasColumnName("gid").HasMaxLength(36);

        // BaseAuditableTable
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(x => x.ModifiedAt).HasColumnName("modified_at");
        builder.Property(x => x.ModifiedByUserId).HasColumnName("modified_by_user_id");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        // Currency
        builder.Property(x => x.CurrencyNamePL).HasColumnName("currency_name_pl").HasMaxLength(120);
        builder.Property(x => x.CurrencyForeignRate).HasColumnName("currency_foreign_rate").HasPrecision(18, 6);

        // Financial
        builder.Property(x => x.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 2);
        builder.Property(x => x.VatRate).HasColumnName("vat_rate").HasPrecision(5, 2);
        builder.Property(x => x.WasPaid).HasColumnName("was_paid");

        // KSE
        builder.Property(x => x.KSERefNumber).HasColumnName("kse_ref_number").HasMaxLength(60);
        builder.Property(x => x.KSEAccountNumber).HasColumnName("kse_account_number").HasMaxLength(60);

        // Document type (no FK, index only)
        builder.Property(x => x.DocumentTypeId).HasColumnName("document_type_id");
        builder.Property(x => x.DocumentTypeNamePL).HasColumnName("document_type_name_pl").HasMaxLength(120);

        // Issuer
        builder.Property(x => x.IssuedBy).HasColumnName("issued_by").HasMaxLength(250);
        builder.Property(x => x.IssuedVatNumber).HasColumnName("issued_vat_number").HasMaxLength(30);

        // Recipient
        builder.Property(x => x.IssuedFor).HasColumnName("issued_for").HasMaxLength(250);
        builder.Property(x => x.IssuedForVatNumber).HasColumnName("issued_for_vat_number").HasMaxLength(30);

        // Indexes
        builder.HasIndex(x => x.Gid).IsUnique();
        builder.HasIndex(x => x.DocumentTypeId);
        builder.HasIndex(x => x.IssuedForVatNumber);
        builder.HasIndex(x => x.CurrencyNamePL);
        builder.HasIndex(x => x.IssuedVatNumber);
        builder.HasIndex(x => x.IsDeleted);
    }
}
