using Compendium.Domain.SharedKernel;
using Compendium.Domain.Sources;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Compendium.Infra.Persistence.Sources;

internal static class SourceEfConversions
{
    public static readonly ValueConverter<CompendiumEntityId, Guid> EntityId =
        new(id => id.Value, value => CompendiumEntityId.Create(value).Value);

    public static readonly ValueConverter<RulesetCode, string> RulesetCode =
        new(code => code.Value, value => Domain.Sources.RulesetCode.Create(value).Value);

    public static readonly ValueConverter<RulesetName, string> RulesetName =
        new(name => name.Value, value => Domain.Sources.RulesetName.Create(value).Value);

    public static readonly ValueConverter<RulesetVersion, string> RulesetVersion =
        new(version => version.Value, value => Domain.Sources.RulesetVersion.Create(value).Value);

    public static readonly ValueConverter<SourceCode, string> SourceCode =
        new(code => code.Value, value => Domain.Sources.SourceCode.Create(value).Value);

    public static readonly ValueConverter<SourceName, string> SourceName =
        new(name => name.Value, value => Domain.Sources.SourceName.Create(value).Value);

    public static readonly ValueConverter<SourceVersionNumber, string> SourceVersionNumber =
        new(version => version.Value, value => Domain.Sources.SourceVersionNumber.Create(value).Value);

    public static readonly ValueConverter<PublicationDate, DateOnly> PublicationDate =
        new(date => date.Value, value => Domain.Sources.PublicationDate.Create(value, DateOnly.MaxValue).Value);
}
