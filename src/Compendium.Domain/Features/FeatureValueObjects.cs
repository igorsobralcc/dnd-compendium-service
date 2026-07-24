using System.Text.RegularExpressions;
using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Features;

public sealed record FeatureCode
{
    public const int MaxLength = 120;

    private FeatureCode(string value) => Value = value;

    public string Value { get; }

    public static Result<FeatureCode> Create(string? value) =>
        FeatureValueObjectFactory.CreateCode(value, "feature-code", static value => new FeatureCode(value), MaxLength);

    public override string ToString() => Value;
}

public sealed record FeatureName
{
    public const int MaxLength = 180;

    private FeatureName(string value) => Value = value;

    public string Value { get; }

    public static Result<FeatureName> Create(string? value) =>
        FeatureValueObjectFactory.CreateText(value, "feature-name", MaxLength, static value => new FeatureName(value));

    public override string ToString() => Value;
}

public sealed record FeatureDescription
{
    public const int MaxLength = 4000;

    private FeatureDescription(string value) => Value = value;

    public string Value { get; }

    public static Result<FeatureDescription?> CreateOptional(string? value)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Result<FeatureDescription?>.Success(null);
        }

        return normalized.Length > MaxLength
            ? Result<FeatureDescription?>.Failure(FeatureDomainErrors.TooLong("feature-description", MaxLength))
            : Result<FeatureDescription?>.Success(new FeatureDescription(normalized));
    }

    public override string ToString() => Value;
}

public sealed record ChoiceSetCode
{
    public const int MaxLength = 120;

    private ChoiceSetCode(string value) => Value = value;

    public string Value { get; }

    public static Result<ChoiceSetCode> Create(string? value) =>
        FeatureValueObjectFactory.CreateCode(value, "choice-set-code", static value => new ChoiceSetCode(value), MaxLength);

    public override string ToString() => Value;
}

public enum CompendiumEntityKind { Feature = 1, Class = 2, ClassLevel = 3, Species = 4, Background = 5, Feat = 6, Spell = 7, ChoiceSet = 8 }
public enum EffectType { GrantProficiency = 1, GrantLanguage = 2, ModifyAbilityScore = 3, GrantFeature = 4, GrantSpell = 5, SetValue = 6 }
public enum EffectTarget { Character = 1, Ability = 2, Skill = 3, Proficiency = 4, Language = 5, Spell = 6, Equipment = 7 }
public enum ConditionType { Always = 1, HasLevel = 2, HasClass = 3, HasFeature = 4, HasChoice = 5 }
public enum EffectValueType { Text = 1, Number = 2, Boolean = 3, Reference = 4, Enum = 5 }
public enum PrerequisiteType { MinimumLevel = 1, AbilityScore = 2, Proficiency = 3, Feature = 4, Class = 5, Spellcasting = 6 }
public enum ComparisonOperator { Exists = 1, Equals = 2, NotEquals = 3, GreaterThanOrEqual = 4, LessThanOrEqual = 5 }
public enum ChoiceOptionType { Proficiency = 1, Language = 2, Spell = 3, Equipment = 4, Feature = 5, Text = 6 }
public enum ChoiceFilterType { ProficiencyType = 1, LanguageType = 2, SpellLevel = 3, EquipmentCategory = 4, SourceEntity = 5 }

file static class FeatureValueObjectFactory
{
    private static readonly Regex CodePattern = new("^[A-Z0-9._-]+$", RegexOptions.Compiled);

    public static Result<T> CreateText<T>(string? value, string field, int maxLength, Func<string, T> factory)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Result<T>.Failure(FeatureDomainErrors.Required(field));
        }

        return normalized.Length > maxLength
            ? Result<T>.Failure(FeatureDomainErrors.TooLong(field, maxLength))
            : Result<T>.Success(factory(normalized));
    }

    public static Result<T> CreateCode<T>(string? value, string field, Func<string, T> factory, int maxLength)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        var text = CreateText(normalized, field, maxLength, static value => value);
        if (text.IsFailure) return Result<T>.Failure(text.Error);

        return CodePattern.IsMatch(text.Value)
            ? Result<T>.Success(factory(text.Value))
            : Result<T>.Failure(FeatureDomainErrors.InvalidCode(field));
    }
}
