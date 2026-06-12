using Compendium.Domain.Fundamentals;
using Compendium.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Compendium.Infra.Persistence.Fundamentals;

internal static class FundamentalEfConversions
{
    public static readonly ValueConverter<CompendiumEntityId, Guid> EntityId =
        new(id => id.Value, value => CompendiumEntityId.Create(value).Value);

    public static readonly ValueConverter<CompendiumEntityId?, Guid?> NullableEntityId =
        new(
            id => id == null ? null : id.Value,
            value => value.HasValue ? CompendiumEntityId.Create(value.Value).Value : null);

    public static readonly ValueConverter<AbilityCode, string> AbilityCode =
        new(code => code.Value, value => Domain.Fundamentals.AbilityCode.Create(value).Value);

    public static readonly ValueConverter<SkillCode, string> SkillCode =
        new(code => code.Value, value => Domain.Fundamentals.SkillCode.Create(value).Value);

    public static readonly ValueConverter<LanguageCode, string> LanguageCode =
        new(code => code.Value, value => Domain.Fundamentals.LanguageCode.Create(value).Value);

    public static readonly ValueConverter<ProficiencyCode, string> ProficiencyCode =
        new(code => code.Value, value => Domain.Fundamentals.ProficiencyCode.Create(value).Value);

    public static readonly ValueConverter<ArmorTrainingCategoryCode, string> ArmorTrainingCategoryCode =
        new(code => code.Value, value => Domain.Fundamentals.ArmorTrainingCategoryCode.Create(value).Value);

    public static readonly ValueConverter<HitDieCode, string> HitDieCode =
        new(code => code.Value, value => ParseHitDieCode(value));

    public static readonly ValueConverter<DisplayName, string> DisplayName =
        new(name => name.Value, value => Domain.Fundamentals.DisplayName.Create(value).Value);

    private static HitDieCode ParseHitDieCode(string value) =>
        Domain.Fundamentals.HitDieCode.Create(int.Parse(value.Substring(1))).Value;
}
