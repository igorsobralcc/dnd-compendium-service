using System.Text.RegularExpressions;
using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Fundamentals;

public sealed record AbilityCode
{
    public const int MaxLength = 40;

    private AbilityCode(string value) => Value = value;

    public string Value { get; }

    public static Result<AbilityCode> Create(string? value) =>
        FundamentalValueObjectFactory.CreateCode(value, "ability-code", v => new AbilityCode(v));

    public override string ToString() => Value;
}

public sealed record SkillCode
{
    public const int MaxLength = 40;

    private SkillCode(string value) => Value = value;

    public string Value { get; }

    public static Result<SkillCode> Create(string? value) =>
        FundamentalValueObjectFactory.CreateCode(value, "skill-code", v => new SkillCode(v));

    public override string ToString() => Value;
}

public sealed record LanguageCode
{
    public const int MaxLength = 40;

    private LanguageCode(string value) => Value = value;

    public string Value { get; }

    public static Result<LanguageCode> Create(string? value) =>
        FundamentalValueObjectFactory.CreateCode(value, "language-code", v => new LanguageCode(v));

    public override string ToString() => Value;
}

public sealed record ProficiencyCode
{
    public const int MaxLength = 80;

    private ProficiencyCode(string value) => Value = value;

    public string Value { get; }

    public static Result<ProficiencyCode> Create(string? value) =>
        FundamentalValueObjectFactory.CreateCode(value, "proficiency-code", v => new ProficiencyCode(v), MaxLength);

    public override string ToString() => Value;
}

public sealed record ArmorTrainingCategoryCode
{
    public const int MaxLength = 40;

    private ArmorTrainingCategoryCode(string value) => Value = value;

    public string Value { get; }

    public static Result<ArmorTrainingCategoryCode> Create(string? value) =>
        FundamentalValueObjectFactory.CreateCode(value, "armor-training-category-code", v => new ArmorTrainingCategoryCode(v));

    public override string ToString() => Value;
}

public sealed record HitDieCode
{
    public const int MaxLength = 10;

    private HitDieCode(string value) => Value = value;

    public string Value { get; }

    public static Result<HitDieCode> Create(int die)
    {
        if (die is not (6 or 8 or 10 or 12))
        {
            return Result<HitDieCode>.Failure(FundamentalDomainErrors.InvalidHitDie(die));
        }

        return Result<HitDieCode>.Success(new HitDieCode($"D{die}"));
    }

    public override string ToString() => Value;
}

public sealed record AbilityScoreMethodCode
{
    public const int MaxLength = 80;

    private AbilityScoreMethodCode(string value) => Value = value;

    public string Value { get; }

    public static Result<AbilityScoreMethodCode> Create(string? value) =>
        FundamentalValueObjectFactory.CreateCode(value, "ability-score-method-code", v => new AbilityScoreMethodCode(v), MaxLength);

    public override string ToString() => Value;
}

public sealed record AbilityScoreMethodRuleCode
{
    public const int MaxLength = 80;

    private AbilityScoreMethodRuleCode(string value) => Value = value;

    public string Value { get; }

    public static Result<AbilityScoreMethodRuleCode> Create(string? value) =>
        FundamentalValueObjectFactory.CreateCode(value, "ability-score-method-rule-code", v => new AbilityScoreMethodRuleCode(v), MaxLength);

    public override string ToString() => Value;
}

public sealed record DisplayName
{
    public const int MaxLength = 180;

    private DisplayName(string value) => Value = value;

    public string Value { get; }

    public static Result<DisplayName> Create(string? value) =>
        FundamentalValueObjectFactory.CreateText(value, "display-name", MaxLength, v => new DisplayName(v));

    public override string ToString() => Value;
}

public enum ProficiencyType
{
    Skill = 1,
    Tool = 2,
    Weapon = 3,
    Armor = 4,
    SavingThrow = 5,
    Language = 6,
    Other = 7
}

public enum AbilityScoreMethodType
{
    StandardArray = 1,
    PointBuy = 2,
    RandomRoll = 3
}

file static class FundamentalValueObjectFactory
{
    private static readonly Regex CodePattern = new("^[A-Z0-9._-]+$", RegexOptions.Compiled);

    public static Result<T> CreateText<T>(string? value, string field, int maxLength, Func<string, T> factory)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Result<T>.Failure(FundamentalDomainErrors.Required(field));
        }

        return normalized.Length > maxLength
            ? Result<T>.Failure(FundamentalDomainErrors.TooLong(field, maxLength))
            : Result<T>.Success(factory(normalized));
    }

    public static Result<T> CreateCode<T>(string? value, string field, Func<string, T> factory, int maxLength = 40)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        var textResult = CreateText(normalized, field, maxLength, static v => v);
        if (textResult.IsFailure)
        {
            return Result<T>.Failure(textResult.Error);
        }

        return CodePattern.IsMatch(textResult.Value)
            ? Result<T>.Success(factory(textResult.Value))
            : Result<T>.Failure(FundamentalDomainErrors.InvalidCode(field));
    }
}
