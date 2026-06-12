using System.Text.RegularExpressions;
using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Sources;

public sealed partial record RulesetCode
{
    public const int MaxLength = 40;

    private RulesetCode(string value) => Value = value;

    public string Value { get; }

    public static Result<RulesetCode> Create(string? value) => SourceValueObjectFactory.CreateCode(value, "ruleset-code", v => new RulesetCode(v));

    public override string ToString() => Value;
}

public sealed record RulesetName
{
    public const int MaxLength = 160;

    private RulesetName(string value) => Value = value;

    public string Value { get; }

    public static Result<RulesetName> Create(string? value) => SourceValueObjectFactory.CreateText(value, "ruleset-name", MaxLength, v => new RulesetName(v));

    public override string ToString() => Value;
}

public sealed record RulesetVersion
{
    public const int MaxLength = 40;

    private RulesetVersion(string value) => Value = value;

    public string Value { get; }

    public static Result<RulesetVersion> Create(string? value) => SourceValueObjectFactory.CreateText(value, "ruleset-version", MaxLength, v => new RulesetVersion(v));

    public override string ToString() => Value;
}

public sealed record SourceCode
{
    public const int MaxLength = 40;

    private SourceCode(string value) => Value = value;

    public string Value { get; }

    public static Result<SourceCode> Create(string? value) => SourceValueObjectFactory.CreateCode(value, "source-code", v => new SourceCode(v));

    public override string ToString() => Value;
}

public sealed record SourceName
{
    public const int MaxLength = 180;

    private SourceName(string value) => Value = value;

    public string Value { get; }

    public static Result<SourceName> Create(string? value) => SourceValueObjectFactory.CreateText(value, "source-name", MaxLength, v => new SourceName(v));

    public override string ToString() => Value;
}

public sealed record SourceVersionNumber
{
    public const int MaxLength = 40;

    private SourceVersionNumber(string value) => Value = value;

    public string Value { get; }

    public static Result<SourceVersionNumber> Create(string? value) => SourceValueObjectFactory.CreateText(value, "source-version-number", MaxLength, v => new SourceVersionNumber(v));

    public override string ToString() => Value;
}

public sealed record PublicationDate
{
    private PublicationDate(DateOnly value) => Value = value;

    public DateOnly Value { get; }

    public static Result<PublicationDate> Create(DateOnly value, DateOnly today)
    {
        return value > today
            ? Result<PublicationDate>.Failure(SourceDomainErrors.InvalidSourceVersionPublicationDate())
            : Result<PublicationDate>.Success(new PublicationDate(value));
    }
}

public enum RulesetStatus
{
    Draft = 1,
    Active = 2,
    Deprecated = 3
}

public enum SourceType
{
    Srd = 1,
    OfficialBook = 2,
    Supplement = 3,
    Homebrew = 4
}

public enum SourceStatus
{
    Inactive = 1,
    Active = 2
}

public enum ImportStatus
{
    Pending = 1,
    Imported = 2,
    Failed = 3
}

public sealed record SourceVersionCreated(
    Guid EventId,
    Guid SourceVersionId,
    Guid RuleSourceId,
    string VersionNumber,
    DateTimeOffset OccurredAtUtc)
{
    public const string EventName = "compendium.source-version-imported.v1";
}

file static class SourceValueObjectFactory
{
    private static readonly Regex CodePattern = new("^[A-Z0-9._-]+$", RegexOptions.Compiled);

    public static Result<T> CreateText<T>(string? value, string field, int maxLength, Func<string, T> factory)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Result<T>.Failure(SourceDomainErrors.Required(field));
        }

        return normalized.Length > maxLength
            ? Result<T>.Failure(SourceDomainErrors.TooLong(field, maxLength))
            : Result<T>.Success(factory(normalized));
    }

    public static Result<T> CreateCode<T>(string? value, string field, Func<string, T> factory)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        var textResult = CreateText(normalized, field, 40, static v => v);
        if (textResult.IsFailure)
        {
            return Result<T>.Failure(textResult.Error);
        }

        return CodePattern.IsMatch(textResult.Value)
            ? Result<T>.Success(factory(textResult.Value))
            : Result<T>.Failure(SourceDomainErrors.InvalidCode(field));
    }
}
