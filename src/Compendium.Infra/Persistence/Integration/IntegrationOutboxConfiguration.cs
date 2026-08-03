using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Integration;

internal sealed class IntegrationOutboxConfiguration : IEntityTypeConfiguration<IntegrationOutbox>
{
    public void Configure(EntityTypeBuilder<IntegrationOutbox> builder)
    {
        builder.ToTable("integration_outbox");
        builder.HasKey(outbox => outbox.Id).HasName("pk_integration_outbox");

        builder.Property(outbox => outbox.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(outbox => outbox.EventId).HasColumnName("event_id").IsRequired();
        builder.Property(outbox => outbox.EventName).HasColumnName("event_name").HasMaxLength(160).IsRequired();
        builder.Property(outbox => outbox.EventVersion).HasColumnName("event_version").IsRequired();
        builder.Property(outbox => outbox.AggregateType).HasColumnName("aggregate_type").HasMaxLength(120).IsRequired();
        builder.Property(outbox => outbox.AggregateId).HasColumnName("aggregate_id").HasMaxLength(128).IsRequired();
        builder.Property(outbox => outbox.CorrelationId).HasColumnName("correlation_id").HasMaxLength(128).IsRequired();
        builder.Property(outbox => outbox.OccurredAtUtc).HasColumnName("occurred_at_utc").IsRequired();
        builder.Property(outbox => outbox.AvailableAtUtc).HasColumnName("available_at_utc").IsRequired();
        builder.Property(outbox => outbox.PublishedAtUtc).HasColumnName("published_at_utc");
        builder.Property(outbox => outbox.Status).HasColumnName("status").HasMaxLength(40).IsRequired();
        builder.Property(outbox => outbox.RetryCount).HasColumnName("retry_count").IsRequired();
        builder.Property(outbox => outbox.LastError).HasColumnName("last_error").HasMaxLength(2000);
        builder.Property(outbox => outbox.ClaimToken).HasColumnName("claim_token").IsConcurrencyToken();
        builder.Property(outbox => outbox.ProcessingOwner).HasColumnName("processing_owner").HasMaxLength(128);
        builder.Property(outbox => outbox.ProcessingStartedAtUtc).HasColumnName("processing_started_at_utc");
        builder.Property(outbox => outbox.LeaseExpiresAtUtc).HasColumnName("lease_expires_at_utc");
        builder.Property(outbox => outbox.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(outbox => outbox.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();

        builder.HasIndex(outbox => outbox.EventId).HasDatabaseName("ux_integration_outbox_event_id").IsUnique();
        builder.HasIndex(outbox => outbox.Status).HasDatabaseName("ix_integration_outbox_status");
        builder.HasIndex(outbox => outbox.AggregateId).HasDatabaseName("ix_integration_outbox_aggregate_id");
        builder.HasIndex(outbox => new { outbox.AvailableAtUtc, outbox.CreatedAtUtc })
            .HasDatabaseName("ix_integration_outbox_active_available_created")
            .HasFilter("status IN ('PENDING', 'FAILED')");
        builder.HasIndex(outbox => outbox.PublishedAtUtc)
            .HasDatabaseName("ix_integration_outbox_published_at")
            .HasFilter("status = 'PUBLISHED'");
        builder.HasIndex(outbox => new { outbox.LeaseExpiresAtUtc, outbox.CreatedAtUtc })
            .HasDatabaseName("ix_integration_outbox_processing_lease")
            .HasFilter("status = 'PROCESSING'");
    }
}
