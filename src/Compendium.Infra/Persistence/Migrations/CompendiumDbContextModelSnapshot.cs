using Compendium.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations;

[DbContext(typeof(CompendiumDbContext))]
partial class CompendiumDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.4")
            .HasDefaultSchema(CompendiumDbContext.Schema);

        modelBuilder.Entity("Compendium.Infra.Persistence.Integration.IntegrationInbox", builder =>
        {
            builder.Property<Guid>("Id")
                .ValueGeneratedNever()
                .HasColumnName("id");

            builder.Property<string>("ConsumerName")
                .IsRequired()
                .HasMaxLength(160)
                .HasColumnName("consumer_name");

            builder.Property<DateTimeOffset>("CreatedAtUtc")
                .HasColumnName("created_at_utc");

            builder.Property<string>("CorrelationId")
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("correlation_id");

            builder.Property<string>("EventId")
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("event_id");

            builder.Property<string>("EventName")
                .IsRequired()
                .HasMaxLength(160)
                .HasColumnName("event_name");

            builder.Property<int>("EventVersion")
                .HasColumnName("event_version");

            builder.Property<string>("LastError")
                .HasMaxLength(2000)
                .HasColumnName("last_error");

            builder.Property<DateTimeOffset?>("ProcessedAtUtc")
                .HasColumnName("processed_at_utc");

            builder.Property<DateTimeOffset>("ReceivedAtUtc")
                .HasColumnName("received_at_utc");

            builder.Property<int>("RetryCount")
                .HasColumnName("retry_count");

            builder.Property<string>("Status")
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("status");

            builder.Property<DateTimeOffset>("UpdatedAtUtc")
                .HasColumnName("updated_at_utc");

            builder.HasKey("Id")
                .HasName("pk_integration_inbox");

            builder.HasIndex("Status")
                .HasDatabaseName("ix_integration_inbox_status");

            builder.HasIndex("EventId", "ConsumerName")
                .IsUnique()
                .HasDatabaseName("ux_integration_inbox_event_consumer");

            builder.ToTable("integration_inbox", CompendiumDbContext.Schema);
        });

        modelBuilder.Entity("Compendium.Infra.Persistence.Integration.IntegrationOutbox", builder =>
        {
            builder.Property<Guid>("Id")
                .ValueGeneratedNever()
                .HasColumnName("id");

            builder.Property<string>("AggregateId")
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("aggregate_id");

            builder.Property<string>("AggregateType")
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnName("aggregate_type");

            builder.Property<DateTimeOffset>("AvailableAtUtc")
                .HasColumnName("available_at_utc");

            builder.Property<DateTimeOffset>("CreatedAtUtc")
                .HasColumnName("created_at_utc");

            builder.Property<string>("CorrelationId")
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("correlation_id");

            builder.Property<Guid>("EventId")
                .HasColumnName("event_id");

            builder.Property<string>("EventName")
                .IsRequired()
                .HasMaxLength(160)
                .HasColumnName("event_name");

            builder.Property<int>("EventVersion")
                .HasColumnName("event_version");

            builder.Property<string>("LastError")
                .HasMaxLength(2000)
                .HasColumnName("last_error");

            builder.Property<DateTimeOffset>("OccurredAtUtc")
                .HasColumnName("occurred_at_utc");

            builder.Property<DateTimeOffset?>("PublishedAtUtc")
                .HasColumnName("published_at_utc");

            builder.Property<int>("RetryCount")
                .HasColumnName("retry_count");

            builder.Property<string>("Status")
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("status");

            builder.Property<DateTimeOffset>("UpdatedAtUtc")
                .HasColumnName("updated_at_utc");

            builder.HasKey("Id")
                .HasName("pk_integration_outbox");

            builder.HasIndex("AggregateId")
                .HasDatabaseName("ix_integration_outbox_aggregate_id");

            builder.HasIndex("EventId")
                .IsUnique()
                .HasDatabaseName("ux_integration_outbox_event_id");

            builder.HasIndex("Status")
                .HasDatabaseName("ix_integration_outbox_status");

            builder.ToTable("integration_outbox", CompendiumDbContext.Schema);
        });

        modelBuilder.Entity("Compendium.Infra.Persistence.Integration.IntegrationOutboxField", builder =>
        {
            builder.Property<Guid>("Id")
                .ValueGeneratedNever()
                .HasColumnName("id");

            builder.Property<bool?>("BooleanValue")
                .HasColumnName("boolean_value");

            builder.Property<DateTimeOffset>("CreatedAtUtc")
                .HasColumnName("created_at_utc");

            builder.Property<string>("EnumValue")
                .HasMaxLength(160)
                .HasColumnName("enum_value");

            builder.Property<string>("FieldName")
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnName("field_name");

            builder.Property<string>("FieldType")
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("field_type");

            builder.Property<decimal?>("NumberValue")
                .HasPrecision(18, 6)
                .HasColumnName("number_value");

            builder.Property<Guid>("OutboxId")
                .HasColumnName("outbox_id");

            builder.Property<string>("ReferenceValue")
                .HasMaxLength(160)
                .HasColumnName("reference_value");

            builder.Property<string>("TextValue")
                .HasMaxLength(4000)
                .HasColumnName("text_value");

            builder.HasKey("Id")
                .HasName("pk_integration_outbox_fields");

            builder.HasIndex("OutboxId")
                .HasDatabaseName("ix_integration_outbox_fields_outbox_id");

            builder.HasIndex("OutboxId", "FieldName")
                .HasDatabaseName("ix_integration_outbox_fields_outbox_field");

            builder.ToTable("integration_outbox_fields", CompendiumDbContext.Schema);
        });

        modelBuilder.Entity("Compendium.Infra.Persistence.Integration.IntegrationOutboxField", builder =>
        {
            builder.HasOne("Compendium.Infra.Persistence.Integration.IntegrationOutbox", "Outbox")
                .WithMany("Fields")
                .HasForeignKey("OutboxId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("fk_integration_outbox_fields_outbox");

            builder.Navigation("Outbox");
        });

        modelBuilder.Entity("Compendium.Infra.Persistence.Integration.IntegrationOutbox", builder =>
        {
            builder.Navigation("Fields");
        });
    }
}
