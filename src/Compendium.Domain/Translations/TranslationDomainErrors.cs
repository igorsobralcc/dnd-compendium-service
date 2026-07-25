using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Translations;

public static class TranslationDomainErrors
{
    public static DomainError InvalidLocale() => new("translation.invalid_locale", "Locale must be a valid BCP 47 language tag.", DomainErrorKind.Validation);
    public static DomainError InvalidField() => new("translation.invalid_field", "Translation field must be a lower snake_case identifier.", DomainErrorKind.Validation);
    public static DomainError InvalidText() => new("translation.invalid_text", "Translated text is required and must contain at most 10000 characters.", DomainErrorKind.Validation);
    public static DomainError InvalidEntityType() => new("translation.invalid_entity_type", "Entity type must be a lower snake_case identifier.", DomainErrorKind.Validation);
}
