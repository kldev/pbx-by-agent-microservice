using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CdrService.Data.Entities;

namespace CdrService.Data.Configurations;

public class CallRecordConfiguration : IEntityTypeConfiguration<CallRecord>
{
    public void Configure(EntityTypeBuilder<CallRecord> builder)
    {
        builder.ToTable("call_records");

        // Primary Key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Gid).HasColumnName("gid").HasMaxLength(36);

        // Call Identification
        builder.Property(x => x.CallUuid).HasColumnName("call_uuid").HasMaxLength(64);
        builder.Property(x => x.CallerId).HasColumnName("caller_id").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CalledNumber).HasColumnName("called_number").HasMaxLength(32).IsRequired();

        // Time Information
        builder.Property(x => x.StartTime).HasColumnName("start_time");
        builder.Property(x => x.AnswerTime).HasColumnName("answer_time");
        builder.Property(x => x.EndTime).HasColumnName("end_time");
        builder.Property(x => x.Duration).HasColumnName("duration");
        builder.Property(x => x.BillableSeconds).HasColumnName("billable_seconds");

        // Status & Result (FK)
        builder.Property(x => x.CallStatusId).HasColumnName("call_status_id");
        builder.HasOne(x => x.CallStatus)
            .WithMany(s => s.CallRecords)
            .HasForeignKey(x => x.CallStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.TerminationCauseId).HasColumnName("termination_cause_id");
        builder.HasOne(x => x.TerminationCause)
            .WithMany(t => t.CallRecords)
            .HasForeignKey(x => x.TerminationCauseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CallDirectionId).HasColumnName("call_direction_id");
        builder.HasOne(x => x.CallDirection)
            .WithMany(d => d.CallRecords)
            .HasForeignKey(x => x.CallDirectionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Gateway Information
        builder.Property(x => x.SourceGatewayGid).HasColumnName("source_gateway_gid").HasMaxLength(36);
        builder.Property(x => x.SourceGatewayName).HasColumnName("source_gateway_name").HasMaxLength(100);
        builder.Property(x => x.DestinationGatewayGid).HasColumnName("destination_gateway_gid").HasMaxLength(36);
        builder.Property(x => x.DestinationGatewayName).HasColumnName("destination_gateway_name").HasMaxLength(100);

        // Snapshot: Tariff/Rate
        builder.Property(x => x.TariffGid).HasColumnName("tariff_gid").HasMaxLength(36);
        builder.Property(x => x.TariffName).HasColumnName("tariff_name").HasMaxLength(100);
        builder.Property(x => x.RatePerMinute).HasColumnName("rate_per_minute").HasPrecision(10, 6);
        builder.Property(x => x.ConnectionFee).HasColumnName("connection_fee").HasPrecision(10, 4);
        builder.Property(x => x.BillingIncrement).HasColumnName("billing_increment");
        builder.Property(x => x.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
        builder.Property(x => x.DestinationName).HasColumnName("destination_name").HasMaxLength(100);
        builder.Property(x => x.MatchedPrefix).HasColumnName("matched_prefix").HasMaxLength(15);

        // Billing
        builder.Property(x => x.TotalCost).HasColumnName("total_cost").HasPrecision(12, 4);

        // Snapshot: Customer
        builder.Property(x => x.CustomerGid).HasColumnName("customer_gid").HasMaxLength(36);
        builder.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(200);
        builder.Property(x => x.SipAccountGid).HasColumnName("sip_account_gid").HasMaxLength(36);
        builder.Property(x => x.SipAccountUsername).HasColumnName("sip_account_username").HasMaxLength(100);

        // Additional
        builder.Property(x => x.UserData).HasColumnName("user_data").HasMaxLength(1000);
        builder.Property(x => x.RawCdrJson).HasColumnName("raw_cdr_json").HasColumnType("text");

        // Audit fields
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(x => x.ModifiedAt).HasColumnName("modified_at");
        builder.Property(x => x.ModifiedByUserId).HasColumnName("modified_by_user_id");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        // Indexes
        builder.HasIndex(x => x.Gid).IsUnique();
        builder.HasIndex(x => x.CallUuid);
        builder.HasIndex(x => x.CallerId);
        builder.HasIndex(x => x.CalledNumber);
        builder.HasIndex(x => x.StartTime);
        builder.HasIndex(x => x.CustomerGid);
        builder.HasIndex(x => x.SipAccountGid);
        builder.HasIndex(x => new { x.StartTime, x.CallStatusId });
        builder.HasIndex(x => new { x.CustomerGid, x.StartTime });
    }
}
