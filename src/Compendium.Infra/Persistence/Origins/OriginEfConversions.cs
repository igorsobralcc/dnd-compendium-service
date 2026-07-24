using Compendium.Domain.Origins;
using Compendium.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Compendium.Infra.Persistence.Origins;

internal static class OriginEfConversions
{
    public static readonly ValueConverter<CompendiumEntityId, Guid> EntityId =
        new(id => id.Value, value => CompendiumEntityId.Create(value).Value);
    public static readonly ValueConverter<SpeciesCode, string> SpeciesCode =
        new(code => code.Value, value => Domain.Origins.SpeciesCode.Create(value).Value);
    public static readonly ValueConverter<SpeciesName, string> SpeciesName =
        new(name => name.Value, value => Domain.Origins.SpeciesName.Create(value).Value);
    public static readonly ValueConverter<SpeciesDescription?, string?> SpeciesDescription =
        new(value => value == null ? null : value.Value, value => Domain.Origins.SpeciesDescription.CreateOptional(value).Value);
    public static readonly ValueConverter<BackgroundCode, string> BackgroundCode =
        new(code => code.Value, value => Domain.Origins.BackgroundCode.Create(value).Value);
    public static readonly ValueConverter<BackgroundName, string> BackgroundName =
        new(name => name.Value, value => Domain.Origins.BackgroundName.Create(value).Value);
    public static readonly ValueConverter<BackgroundDescription?, string?> BackgroundDescription =
        new(value => value == null ? null : value.Value, value => Domain.Origins.BackgroundDescription.CreateOptional(value).Value);
    public static readonly ValueConverter<FeatCode, string> FeatCode =
        new(code => code.Value, value => Domain.Origins.FeatCode.Create(value).Value);
    public static readonly ValueConverter<FeatName, string> FeatName =
        new(name => name.Value, value => Domain.Origins.FeatName.Create(value).Value);
    public static readonly ValueConverter<FeatDescription?, string?> FeatDescription =
        new(value => value == null ? null : value.Value, value => Domain.Origins.FeatDescription.CreateOptional(value).Value);
}
