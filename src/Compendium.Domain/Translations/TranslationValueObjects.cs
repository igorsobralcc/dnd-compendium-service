using System.Text.RegularExpressions;
using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Translations;

public sealed record Locale
{
    public const int MaxLength = 35;
    private static readonly Regex Pattern = new(
        "^[a-zA-Z]{2,3}(?:-[a-zA-Z]{4})?(?:-[a-zA-Z]{2}|-[0-9]{3})?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private Locale(string value) => Value = value;

    public string Value { get; }

    public static Result<Locale> Create(string? value)
    {
        var normalized = value?.Trim().Replace('_', '-');
        if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > MaxLength || !Pattern.IsMatch(normalized))
            return Result<Locale>.Failure(TranslationDomainErrors.InvalidLocale());

        var parts = normalized.Split('-');
        parts[0] = parts[0].ToLowerInvariant();
        for (var index = 1; index < parts.Length; index++)
            parts[index] = parts[index].Length == 2
                ? parts[index].ToUpperInvariant()
                : char.ToUpperInvariant(parts[index][0]) + parts[index][1..].ToLowerInvariant();

        return Result<Locale>.Success(new Locale(string.Join('-', parts)));
    }

    public override string ToString() => Value;
}

public sealed record TranslationField
{
    public const int MaxLength = 80;
    private static readonly Regex Pattern = new(
        "^[a-z][a-z0-9]*(?:_[a-z0-9]+)*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private TranslationField(string value) => Value = value;
    public string Value { get; }

    public static Result<TranslationField> Create(string? value)
    {
        var normalized = value?.Trim().ToLowerInvariant();
        return string.IsNullOrWhiteSpace(normalized) || normalized.Length > MaxLength || !Pattern.IsMatch(normalized)
            ? Result<TranslationField>.Failure(TranslationDomainErrors.InvalidField())
            : Result<TranslationField>.Success(new TranslationField(normalized));
    }
}

public sealed record TranslatedText
{
    public const int MaxLength = 10000;
    private TranslatedText(string value) => Value = value;
    public string Value { get; }

    public static Result<TranslatedText> Create(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) || normalized.Length > MaxLength
            ? Result<TranslatedText>.Failure(TranslationDomainErrors.InvalidText())
            : Result<TranslatedText>.Success(new TranslatedText(normalized));
    }
}

public sealed record TranslatableEntityType
{
    public const int MaxLength = 80;
    private static readonly Regex Pattern = new(
        "^[a-z][a-z0-9]*(?:_[a-z0-9]+)*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private TranslatableEntityType(string value) => Value = value;
    public string Value { get; }

    public static Result<TranslatableEntityType> Create(string? value)
    {
        var normalized = value?.Trim().ToLowerInvariant();
        return string.IsNullOrWhiteSpace(normalized) || normalized.Length > MaxLength || !Pattern.IsMatch(normalized)
            ? Result<TranslatableEntityType>.Failure(TranslationDomainErrors.InvalidEntityType())
            : Result<TranslatableEntityType>.Success(new TranslatableEntityType(normalized));
    }
}
