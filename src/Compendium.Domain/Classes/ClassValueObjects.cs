using System.Text.RegularExpressions;
using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Classes;

public sealed record ClassCode
{
    public const int MaxLength = 80;

    private ClassCode(string value) => Value = value;

    public string Value { get; }

    public static Result<ClassCode> Create(string? value) =>
        ClassValueObjectFactory.CreateCode(value, "class-code", v => new ClassCode(v), MaxLength);

    public override string ToString() => Value;
}

public sealed record ClassName
{
    public const int MaxLength = 180;

    private ClassName(string value) => Value = value;

    public string Value { get; }

    public static Result<ClassName> Create(string? value) =>
        ClassValueObjectFactory.CreateText(value, "class-name", MaxLength, v => new ClassName(v));

    public override string ToString() => Value;
}

public sealed record ClassDescription
{
    public const int MaxLength = 4000;

    private ClassDescription(string value) => Value = value;

    public string Value { get; }

    public static Result<ClassDescription?> CreateOptional(string? value)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Result<ClassDescription?>.Success(null);
        }

        return normalized.Length > MaxLength
            ? Result<ClassDescription?>.Failure(ClassDomainErrors.TooLong("class-description", MaxLength))
            : Result<ClassDescription?>.Success(new ClassDescription(normalized));
    }

    public override string ToString() => Value;
}

public enum ClassSpellcastingProgressionType
{
    FullCaster = 1,
    HalfCaster = 2,
    ThirdCaster = 3,
    PactMagic = 4
}

file static class ClassValueObjectFactory
{
    private static readonly Regex CodePattern = new("^[A-Z0-9._-]+$", RegexOptions.Compiled);

    public static Result<T> CreateText<T>(string? value, string field, int maxLength, Func<string, T> factory)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Result<T>.Failure(ClassDomainErrors.Required(field));
        }

        return normalized.Length > maxLength
            ? Result<T>.Failure(ClassDomainErrors.TooLong(field, maxLength))
            : Result<T>.Success(factory(normalized));
    }

    public static Result<T> CreateCode<T>(string? value, string field, Func<string, T> factory, int maxLength)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        var textResult = CreateText(normalized, field, maxLength, static v => v);
        if (textResult.IsFailure)
        {
            return Result<T>.Failure(textResult.Error);
        }

        return CodePattern.IsMatch(textResult.Value)
            ? Result<T>.Success(factory(textResult.Value))
            : Result<T>.Failure(ClassDomainErrors.InvalidCode(field));
    }
}
