using Compendium.Domain.SharedKernel;
using Compendium.Domain.Translations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Compendium.Infra.Persistence.Translations;

internal sealed class TranslationConfiguration : IEntityTypeConfiguration<Translation>
{
    private static readonly ValueConverter<CompendiumEntityId, Guid> EntityId =
        new(id => id.Value, value => CompendiumEntityId.Create(value).Value);
    private static readonly ValueConverter<TranslatableEntityType, string> EntityType =
        new(value => value.Value, value => TranslatableEntityType.Create(value).Value);
    private static readonly ValueConverter<Locale, string> Locale =
        new(value => value.Value, value => Domain.Translations.Locale.Create(value).Value);
    private static readonly ValueConverter<TranslationField, string> Field =
        new(value => value.Value, value => TranslationField.Create(value).Value);
    private static readonly ValueConverter<TranslatedText, string> Text =
        new(value => value.Value, value => TranslatedText.Create(value).Value);

    public void Configure(EntityTypeBuilder<Translation> builder)
    {
        builder.ToTable("translations", CompendiumDbContext.Schema);
        builder.HasKey(x => x.Id).HasName("pk_translations");
        builder.Property(x => x.Id).HasConversion(EntityId).ValueGeneratedNever().HasColumnName("id");
        builder.Property(x => x.EntityType).HasConversion(EntityType).HasMaxLength(TranslatableEntityType.MaxLength).HasColumnName("entity_type").IsRequired();
        builder.Property(x => x.EntityId).HasConversion(EntityId).HasColumnName("entity_id").IsRequired();
        builder.Property(x => x.Locale).HasConversion(Locale).HasMaxLength(Domain.Translations.Locale.MaxLength).HasColumnName("locale").IsRequired();
        builder.Property(x => x.Field).HasConversion(Field).HasMaxLength(TranslationField.MaxLength).HasColumnName("field").IsRequired();
        builder.Property(x => x.Text).HasConversion(Text).HasMaxLength(TranslatedText.MaxLength).HasColumnName("translated_text").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();
        builder.HasIndex(x => new { x.EntityType, x.EntityId, x.Locale, x.Field })
            .IsUnique().HasDatabaseName("ux_translations_entity_locale_field");
        builder.HasIndex(x => new { x.EntityType, x.EntityId })
            .HasDatabaseName("ix_translations_entity");
    }
}
