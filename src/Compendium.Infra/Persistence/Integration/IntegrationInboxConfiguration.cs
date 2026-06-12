using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Integration;

internal sealed class IntegrationInboxConfiguration : IEntityTypeConfiguration<IntegrationInbox>
{
    public void Configure(EntityTypeBuilder<IntegrationInbox> builder)
    {
        builder.ToTable("integration_inbox");
        builder.HasKey(inbox => inbox.Id).HasName("pk_integration_inbox");

        builder.Property(inbox => inbox.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(inbox => inbox.EventId).HasColumnName("event_id").HasMaxLength(128).IsRequired();
        builder.Property(inbox => inbox.ConsumerName).HasColumnName("consumer_name").HasMaxLength(160).IsRequired();
        builder.Property(inbox => inbox.EventName).HasColumnName("event_name").HasMaxLength(160).IsRequired();
        builder.Property(inbox => inbox.EventVersion).HasColumnName("event_version").IsRequired();
        builder.Property(inbox => inbox.CorrelationId).HasColumnName("correlation_id").HasMaxLength(128).IsRequired();
        builder.Property(inbox => inbox.ReceivedAtUtc).HasColumnName("received_at_utc").IsRequired();
        builder.Property(inbox => inbox.ProcessedAtUtc).HasColumnName("processed_at_utc");
        builder.Property(inbox => inbox.Status).HasColumnName("status").HasMaxLength(40).IsRequired();
        builder.Property(inbox => inbox.RetryCount).HasColumnName("retry_count").IsRequired();
        builder.Property(inbox => inbox.LastError).HasColumnName("last_error").HasMaxLength(2000);
        builder.Property(inbox => inbox.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(inbox => inbox.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();

        builder.HasIndex(inbox => new { inbox.EventId, inbox.ConsumerName })
            .HasDatabaseName("ux_integration_inbox_event_consumer")
            .IsUnique();

        builder.HasIndex(inbox => inbox.Status).HasDatabaseName("ix_integration_inbox_status");
    }
}
