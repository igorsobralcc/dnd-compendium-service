using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Features;

public sealed class EntityPrerequisite
{
    private EntityPrerequisite()
    {
        EntityId = null!;
        Value = null!;
    }

    private EntityPrerequisite(CompendiumEntityId id, CompendiumEntityKind entityKind, CompendiumEntityId entityId, PrerequisiteType type, ComparisonOperator comparisonOperator, EffectTarget target, TypedMechanicalValue value)
    {
        Id = id;
        EntityKind = entityKind;
        EntityId = entityId;
        Type = type;
        Operator = comparisonOperator;
        Target = target;
        Value = value;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityKind EntityKind { get; private set; }
    public CompendiumEntityId EntityId { get; private set; }
    public PrerequisiteType Type { get; private set; }
    public ComparisonOperator Operator { get; private set; }
    public EffectTarget Target { get; private set; }
    public TypedMechanicalValue Value { get; private set; }

    public static Result<EntityPrerequisite> Create(
        CompendiumEntityKind entityKind,
        CompendiumEntityId entityId,
        PrerequisiteType type,
        ComparisonOperator comparisonOperator,
        EffectTarget target,
        EffectValueType valueType,
        string? textValue,
        decimal? numericValue,
        bool? booleanValue,
        CompendiumEntityId? referenceId,
        string? enumValue)
    {
        if (!Enum.IsDefined(entityKind)) return Result<EntityPrerequisite>.Failure(FeatureDomainErrors.InvalidEnum("entity-kind"));
        if (!Enum.IsDefined(type)) return Result<EntityPrerequisite>.Failure(FeatureDomainErrors.InvalidEnum("prerequisite-type"));
        if (!Enum.IsDefined(comparisonOperator)) return Result<EntityPrerequisite>.Failure(FeatureDomainErrors.InvalidEnum("comparison-operator"));
        if (!Enum.IsDefined(target)) return Result<EntityPrerequisite>.Failure(FeatureDomainErrors.InvalidEnum("prerequisite-target"));

        var value = TypedMechanicalValue.Create(valueType, textValue, numericValue, booleanValue, referenceId, enumValue, "prerequisite");
        return value.IsFailure
            ? Result<EntityPrerequisite>.Failure(value.Error)
            : Result<EntityPrerequisite>.Success(new EntityPrerequisite(CompendiumEntityId.New(), entityKind, entityId, type, comparisonOperator, target, value.Value));
    }
}
