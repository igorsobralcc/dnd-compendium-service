using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compendium.Infra.Persistence.Integration;

internal sealed class IntegrationOutboxFieldConfiguration : IEntityTypeConfiguration<IntegrationOutboxField>
{
    public void Configure(EntityTypeBuilder<IntegrationOutboxField> builder)
    {
        builder.ToTable("integration_outbox_fields");
        builder.HasKey(field => field.Id).HasName("pk_integration_outbox_fields");

        builder.Property(field => field.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(field => field.OutboxId).HasColumnName("outbox_id").IsRequired();
        builder.Property(field => field.FieldName).HasColumnName("field_name").HasMaxLength(120).IsRequired();
        builder.Property(field => field.FieldType).HasColumnName("field_type").HasMaxLength(40).IsRequired();
        builder.Property(field => field.TextValue).HasColumnName("text_value").HasMaxLength(4000);
        builder.Property(field => field.NumberValue).HasColumnName("number_value").HasPrecision(18, 6);
        builder.Property(field => field.BooleanValue).HasColumnName("boolean_value");
        builder.Property(field => field.ReferenceValue).HasColumnName("reference_value").HasMaxLength(160);
        builder.Property(field => field.EnumValue).HasColumnName("enum_value").HasMaxLength(160);
        builder.Property(field => field.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();

        builder
            .HasOne(field => field.Outbox)
            .WithMany(outbox => outbox.Fields)
            .HasForeignKey(field => field.OutboxId)
            .HasConstraintName("fk_integration_outbox_fields_outbox")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(field => field.OutboxId).HasDatabaseName("ix_integration_outbox_fields_outbox_id");
        builder.HasIndex(field => new { field.OutboxId, field.FieldName })
            .HasDatabaseName("ix_integration_outbox_fields_outbox_field");
    }
}
