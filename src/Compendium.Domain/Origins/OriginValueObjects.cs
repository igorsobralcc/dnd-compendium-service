using System.Text.RegularExpressions;
using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Origins;

public sealed record SpeciesCode
{
    public const int MaxLength = 120;
    private SpeciesCode(string value) => Value = value;
    public string Value { get; }
    public static Result<SpeciesCode> Create(string? value) =>
        OriginValueObjectFactory.CreateCode(value, "species-code", MaxLength, static value => new SpeciesCode(value));
    public override string ToString() => Value;
}

public sealed record SpeciesName
{
    public const int MaxLength = 180;
    private SpeciesName(string value) => Value = value;
    public string Value { get; }
    public static Result<SpeciesName> Create(string? value) =>
        OriginValueObjectFactory.CreateText(value, "species-name", MaxLength, static value => new SpeciesName(value));
    public override string ToString() => Value;
}

public sealed record SpeciesDescription
{
    public const int MaxLength = 4000;
    private SpeciesDescription(string value) => Value = value;
    public string Value { get; }
    public static Result<SpeciesDescription?> CreateOptional(string? value) =>
        OriginValueObjectFactory.CreateOptionalText(value, "species-description", MaxLength, static value => new SpeciesDescription(value));
    public override string ToString() => Value;
}

public sealed record BackgroundCode
{
    public const int MaxLength = 120;
    private BackgroundCode(string value) => Value = value;
    public string Value { get; }
    public static Result<BackgroundCode> Create(string? value) =>
        OriginValueObjectFactory.CreateCode(value, "background-code", MaxLength, static value => new BackgroundCode(value));
    public override string ToString() => Value;
}

public sealed record BackgroundName
{
    public const int MaxLength = 180;
    private BackgroundName(string value) => Value = value;
    public string Value { get; }
    public static Result<BackgroundName> Create(string? value) =>
        OriginValueObjectFactory.CreateText(value, "background-name", MaxLength, static value => new BackgroundName(value));
    public override string ToString() => Value;
}

public sealed record BackgroundDescription
{
    public const int MaxLength = 4000;
    private BackgroundDescription(string value) => Value = value;
    public string Value { get; }
    public static Result<BackgroundDescription?> CreateOptional(string? value) =>
        OriginValueObjectFactory.CreateOptionalText(value, "background-description", MaxLength, static value => new BackgroundDescription(value));
    public override string ToString() => Value;
}

public sealed record FeatCode
{
    public const int MaxLength = 120;
    private FeatCode(string value) => Value = value;
    public string Value { get; }
    public static Result<FeatCode> Create(string? value) =>
        OriginValueObjectFactory.CreateCode(value, "feat-code", MaxLength, static value => new FeatCode(value));
    public override string ToString() => Value;
}

public sealed record FeatName
{
    public const int MaxLength = 180;
    private FeatName(string value) => Value = value;
    public string Value { get; }
    public static Result<FeatName> Create(string? value) =>
        OriginValueObjectFactory.CreateText(value, "feat-name", MaxLength, static value => new FeatName(value));
    public override string ToString() => Value;
}

public sealed record FeatDescription
{
    public const int MaxLength = 4000;
    private FeatDescription(string value) => Value = value;
    public string Value { get; }
    public static Result<FeatDescription?> CreateOptional(string? value) =>
        OriginValueObjectFactory.CreateOptionalText(value, "feat-description", MaxLength, static value => new FeatDescription(value));
    public override string ToString() => Value;
}

public enum FeatCategory { Origin = 1, General = 2, FightingStyle = 3, EpicBoon = 4 }
public enum StartingEquipmentReferenceType { Rule = 1, Group = 2 }

file static class OriginValueObjectFactory
{
    private static readonly Regex CodePattern = new("^[A-Z0-9._-]+$", RegexOptions.Compiled);

    public static Result<T> CreateText<T>(string? value, string field, int maxLength, Func<string, T> factory)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Result<T>.Failure(OriginDomainErrors.Required(field));
        }

        return normalized.Length > maxLength
            ? Result<T>.Failure(OriginDomainErrors.TooLong(field, maxLength))
            : Result<T>.Success(factory(normalized));
    }

    public static Result<T?> CreateOptionalText<T>(string? value, string field, int maxLength, Func<string, T> factory)
        where T : class
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Result<T?>.Success(null);
        }

        return normalized.Length > maxLength
            ? Result<T?>.Failure(OriginDomainErrors.TooLong(field, maxLength))
            : Result<T?>.Success(factory(normalized));
    }

    public static Result<T> CreateCode<T>(string? value, string field, int maxLength, Func<string, T> factory)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        var text = CreateText(normalized, field, maxLength, static value => value);
        if (text.IsFailure) return Result<T>.Failure(text.Error);
        return CodePattern.IsMatch(text.Value)
            ? Result<T>.Success(factory(text.Value))
            : Result<T>.Failure(OriginDomainErrors.InvalidCode(field));
    }
}
