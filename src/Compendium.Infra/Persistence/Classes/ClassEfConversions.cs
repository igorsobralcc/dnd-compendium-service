using Compendium.Domain.Classes;
using Compendium.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Compendium.Infra.Persistence.Classes;

internal static class ClassEfConversions
{
    public static readonly ValueConverter<CompendiumEntityId, Guid> EntityId =
        new(id => id.Value, value => CompendiumEntityId.Create(value).Value);

    public static readonly ValueConverter<CompendiumEntityId?, Guid?> NullableEntityId =
        new(
            id => id == null ? null : id.Value,
            value => value.HasValue ? CompendiumEntityId.Create(value.Value).Value : null);

    public static readonly ValueConverter<ClassCode, string> ClassCode =
        new(code => code.Value, value => Domain.Classes.ClassCode.Create(value).Value);

    public static readonly ValueConverter<ClassName, string> ClassName =
        new(name => name.Value, value => Domain.Classes.ClassName.Create(value).Value);

    public static readonly ValueConverter<ClassDescription?, string?> NullableClassDescription =
        new(description => description == null ? null : description.Value, value => Domain.Classes.ClassDescription.CreateOptional(value).Value);
}
