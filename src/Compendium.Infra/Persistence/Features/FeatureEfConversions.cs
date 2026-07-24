using Compendium.Domain.Features;
using Compendium.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Compendium.Infra.Persistence.Features;

internal static class FeatureEfConversions
{
    public static readonly ValueConverter<CompendiumEntityId, Guid> EntityId =
        new(id => id.Value, value => CompendiumEntityId.Create(value).Value);

    public static readonly ValueConverter<CompendiumEntityId?, Guid?> NullableEntityId =
        new(id => id == null ? null : id.Value, value => value.HasValue ? CompendiumEntityId.Create(value.Value).Value : null);

    public static readonly ValueConverter<FeatureCode, string> FeatureCode =
        new(code => code.Value, value => Domain.Features.FeatureCode.Create(value).Value);

    public static readonly ValueConverter<FeatureName, string> FeatureName =
        new(name => name.Value, value => Domain.Features.FeatureName.Create(value).Value);

    public static readonly ValueConverter<FeatureDescription?, string?> NullableFeatureDescription =
        new(description => description == null ? null : description.Value, value => Domain.Features.FeatureDescription.CreateOptional(value).Value);

    public static readonly ValueConverter<ChoiceSetCode, string> ChoiceSetCode =
        new(code => code.Value, value => Domain.Features.ChoiceSetCode.Create(value).Value);
}
