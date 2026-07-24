using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Features;

public sealed class EffectSchema
{
    private readonly List<EffectSchemaField> fields = [];

    private EffectSchema()
    {
        Code = null!;
        Name = null!;
    }

    private EffectSchema(CompendiumEntityId id, FeatureCode code, FeatureName name, EffectType type)
    {
        Id = id;
        Code = code;
        Name = name;
        Type = type;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public FeatureCode Code { get; private set; }
    public FeatureName Name { get; private set; }
    public EffectType Type { get; private set; }
    public IReadOnlyCollection<EffectSchemaField> Fields => fields;

    public static Result<EffectSchema> Create(FeatureCode code, FeatureName name, EffectType type, IReadOnlyCollection<EffectSchemaFieldInput> fieldInputs)
    {
        if (!Enum.IsDefined(type))
        {
            return Result<EffectSchema>.Failure(FeatureDomainErrors.InvalidEnum("effect-type"));
        }

        var schema = new EffectSchema(CompendiumEntityId.New(), code, name, type);
        var seen = new HashSet<string>();
        var sortOrder = 1;
        foreach (var input in fieldInputs)
        {
            var fieldCode = FeatureCode.Create(input.Code);
            if (fieldCode.IsFailure) return Result<EffectSchema>.Failure(fieldCode.Error);
            if (!seen.Add(fieldCode.Value.Value)) return Result<EffectSchema>.Failure(FeatureDomainErrors.DuplicateEffectField(fieldCode.Value.Value));
            if (!Enum.IsDefined(input.ValueType)) return Result<EffectSchema>.Failure(FeatureDomainErrors.InvalidEnum("effect-value-type"));

            schema.fields.Add(EffectSchemaField.Create(CompendiumEntityId.New(), schema.Id, fieldCode.Value, input.ValueType, input.IsRequired, sortOrder++));
        }

        return Result<EffectSchema>.Success(schema);
    }
}

public sealed class EffectSchemaField
{
    private EffectSchemaField()
    {
        EffectSchemaId = null!;
        Code = null!;
    }

    private EffectSchemaField(CompendiumEntityId id, CompendiumEntityId effectSchemaId, FeatureCode code, EffectValueType valueType, bool isRequired, int sortOrder)
    {
        Id = id;
        EffectSchemaId = effectSchemaId;
        Code = code;
        ValueType = valueType;
        IsRequired = isRequired;
        SortOrder = sortOrder;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId EffectSchemaId { get; private set; }
    public FeatureCode Code { get; private set; }
    public EffectValueType ValueType { get; private set; }
    public bool IsRequired { get; private set; }
    public int SortOrder { get; private set; }

    public static EffectSchemaField Create(CompendiumEntityId id, CompendiumEntityId effectSchemaId, FeatureCode code, EffectValueType valueType, bool isRequired, int sortOrder) =>
        new(id, effectSchemaId, code, valueType, isRequired, sortOrder);
}

public sealed class FeatureEffect
{
    private readonly List<FeatureEffectFieldValue> fieldValues = [];
    private readonly List<FeatureEffectCondition> conditions = [];

    private FeatureEffect()
    {
        FeatureId = null!;
        EffectSchemaId = null!;
    }

    private FeatureEffect(CompendiumEntityId id, CompendiumEntityId featureId, CompendiumEntityId effectSchemaId, EffectType type, EffectTarget target, DateTimeOffset createdAtUtc)
    {
        Id = id;
        FeatureId = featureId;
        EffectSchemaId = effectSchemaId;
        Type = type;
        Target = target;
        CreatedAtUtc = createdAtUtc;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId FeatureId { get; private set; }
    public CompendiumEntityId EffectSchemaId { get; private set; }
    public EffectType Type { get; private set; }
    public EffectTarget Target { get; private set; }
    public IReadOnlyCollection<FeatureEffectFieldValue> FieldValues => fieldValues;
    public IReadOnlyCollection<FeatureEffectCondition> Conditions => conditions;
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static Result<FeatureEffect> Create(CompendiumEntityId featureId, EffectSchema schema, FeatureEffectInput input, DateTimeOffset now)
    {
        if (!Enum.IsDefined(input.Type)) return Result<FeatureEffect>.Failure(FeatureDomainErrors.InvalidEnum("effect-type"));
        if (!Enum.IsDefined(input.Target)) return Result<FeatureEffect>.Failure(FeatureDomainErrors.InvalidEnum("effect-target"));
        if (input.Type != schema.Type) return Result<FeatureEffect>.Failure(FeatureDomainErrors.InvalidEnum("effect-type"));

        var effect = new FeatureEffect(CompendiumEntityId.New(), featureId, schema.Id, input.Type, input.Target, now);
        var schemaFields = schema.Fields.ToDictionary(field => field.Code.Value);
        var seen = new HashSet<string>();

        foreach (var inputField in input.Fields)
        {
            var code = FeatureCode.Create(inputField.FieldCode);
            if (code.IsFailure) return Result<FeatureEffect>.Failure(code.Error);
            if (!seen.Add(code.Value.Value)) return Result<FeatureEffect>.Failure(FeatureDomainErrors.DuplicateEffectField(code.Value.Value));
            if (!schemaFields.TryGetValue(code.Value.Value, out var schemaField)) return Result<FeatureEffect>.Failure(FeatureDomainErrors.UnknownEffectField(code.Value.Value));

            var value = TypedMechanicalValue.Create(schemaField.ValueType, inputField.TextValue, inputField.NumericValue, inputField.BooleanValue, inputField.ReferenceId, inputField.EnumValue, "effect-field");
            if (value.IsFailure) return Result<FeatureEffect>.Failure(FeatureDomainErrors.EffectFieldTypeMismatch(code.Value.Value));

            effect.fieldValues.Add(FeatureEffectFieldValue.Create(CompendiumEntityId.New(), effect.Id, schemaField.Id, value.Value));
        }

        foreach (var required in schema.Fields.Where(field => field.IsRequired))
        {
            if (!seen.Contains(required.Code.Value))
            {
                return Result<FeatureEffect>.Failure(FeatureDomainErrors.MissingRequiredEffectField(required.Code.Value));
            }
        }

        foreach (var conditionInput in input.Conditions)
        {
            var condition = FeatureEffectCondition.Create(effect.Id, conditionInput);
            if (condition.IsFailure) return Result<FeatureEffect>.Failure(condition.Error);
            effect.conditions.Add(condition.Value);
        }

        return Result<FeatureEffect>.Success(effect);
    }
}

public sealed class FeatureEffectFieldValue
{
    private FeatureEffectFieldValue()
    {
        FeatureEffectId = null!;
        EffectSchemaFieldId = null!;
        Value = null!;
    }

    private FeatureEffectFieldValue(CompendiumEntityId id, CompendiumEntityId featureEffectId, CompendiumEntityId effectSchemaFieldId, TypedMechanicalValue value)
    {
        Id = id;
        FeatureEffectId = featureEffectId;
        EffectSchemaFieldId = effectSchemaFieldId;
        Value = value;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId FeatureEffectId { get; private set; }
    public CompendiumEntityId EffectSchemaFieldId { get; private set; }
    public TypedMechanicalValue Value { get; private set; }

    public static FeatureEffectFieldValue Create(CompendiumEntityId id, CompendiumEntityId featureEffectId, CompendiumEntityId effectSchemaFieldId, TypedMechanicalValue value) =>
        new(id, featureEffectId, effectSchemaFieldId, value);
}

public sealed class FeatureEffectCondition
{
    private FeatureEffectCondition()
    {
        FeatureEffectId = null!;
        Value = null!;
    }

    private FeatureEffectCondition(CompendiumEntityId id, CompendiumEntityId featureEffectId, ConditionType type, TypedMechanicalValue value)
    {
        Id = id;
        FeatureEffectId = featureEffectId;
        Type = type;
        Value = value;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId FeatureEffectId { get; private set; }
    public ConditionType Type { get; private set; }
    public TypedMechanicalValue Value { get; private set; }

    public static Result<FeatureEffectCondition> Create(CompendiumEntityId featureEffectId, FeatureEffectConditionInput input)
    {
        if (!Enum.IsDefined(input.Type)) return Result<FeatureEffectCondition>.Failure(FeatureDomainErrors.InvalidEnum("condition-type"));

        var value = TypedMechanicalValue.Create(input.ValueType, input.TextValue, input.NumericValue, input.BooleanValue, input.ReferenceId, input.EnumValue, "effect-condition");
        return value.IsFailure
            ? Result<FeatureEffectCondition>.Failure(value.Error)
            : Result<FeatureEffectCondition>.Success(new FeatureEffectCondition(CompendiumEntityId.New(), featureEffectId, input.Type, value.Value));
    }
}

public sealed record EffectSchemaFieldInput(string Code, EffectValueType ValueType, bool IsRequired);
