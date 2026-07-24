using Compendium.Domain.Features;
using Compendium.Domain.SharedKernel;

namespace Compendium.UnitTests.Features;

public sealed class FeatureTests
{
    [Fact]
    public void Feature_code_normalizes_to_uppercase()
    {
        var code = FeatureCode.Create("fighting-style.defense");

        Assert.True(code.IsSuccess);
        Assert.Equal("FIGHTING-STYLE.DEFENSE", code.Value.Value);
    }

    [Fact]
    public void Feature_rejects_negative_level_requirement()
    {
        var feature = CreateFeature(levelRequirement: -1);

        Assert.True(feature.IsFailure);
        Assert.Equal("compendium.features.level-requirement.invalid", feature.Error.Code);
    }

    [Fact]
    public void Feature_effect_requires_schema_fields()
    {
        var feature = CreateFeature().Value;
        var schema = EffectSchema.Create(
            FeatureCode.Create("GRANT-PROFICIENCY").Value,
            FeatureName.Create("Grant proficiency").Value,
            EffectType.GrantProficiency,
            [new EffectSchemaFieldInput("PROFICIENCY", EffectValueType.Reference, true)]).Value;

        var result = feature.AttachEffect(
            schema,
            new FeatureEffectInput(
                EffectType.GrantProficiency,
                EffectTarget.Proficiency,
                [],
                []),
            DateTimeOffset.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal("compendium.features.effect-field.required", result.Error.Code);
    }

    [Fact]
    public void Feature_effect_validates_typed_field_against_schema()
    {
        var feature = CreateFeature().Value;
        var schema = EffectSchema.Create(
            FeatureCode.Create("ABILITY-BONUS").Value,
            FeatureName.Create("Ability bonus").Value,
            EffectType.ModifyAbilityScore,
            [new EffectSchemaFieldInput("BONUS", EffectValueType.Number, true)]).Value;

        var result = feature.AttachEffect(
            schema,
            new FeatureEffectInput(
                EffectType.ModifyAbilityScore,
                EffectTarget.Ability,
                [new FeatureEffectFieldInput("BONUS", "two", null, null, null, null)],
                []),
            DateTimeOffset.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal("compendium.features.effect-field.type-mismatch", result.Error.Code);
    }

    [Fact]
    public void Choice_set_rejects_minimum_greater_than_maximum()
    {
        var choiceSet = ChoiceSet.Create(
            CompendiumEntityKind.Feature,
            CompendiumEntityId.New(),
            ChoiceSetCode.Create("SKILL-CHOICES").Value,
            3,
            2);

        Assert.True(choiceSet.IsFailure);
        Assert.Equal("compendium.features.choice-cardinality.invalid", choiceSet.Error.Code);
    }

    [Fact]
    public void Prerequisite_requires_value_matching_declared_type()
    {
        var prerequisite = EntityPrerequisite.Create(
            CompendiumEntityKind.Feat,
            CompendiumEntityId.New(),
            PrerequisiteType.MinimumLevel,
            ComparisonOperator.GreaterThanOrEqual,
            EffectTarget.Character,
            EffectValueType.Number,
            "five",
            null,
            null,
            null,
            null);

        Assert.True(prerequisite.IsFailure);
        Assert.Equal("compendium.features.prerequisite.typed-value.required", prerequisite.Error.Code);
    }

    private static Result<Feature> CreateFeature(int? levelRequirement = null) =>
        Feature.Create(
            CompendiumEntityId.New(),
            CompendiumEntityId.New(),
            FeatureCode.Create("ACTION-SURGE").Value,
            FeatureName.Create("Action Surge").Value,
            FeatureDescription.CreateOptional("Take one additional action.").Value,
            levelRequirement,
            DateTimeOffset.UtcNow);
}
