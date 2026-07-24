using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Features;

public sealed class TypedMechanicalValue
{
    private TypedMechanicalValue()
    {
    }

    private TypedMechanicalValue(
        EffectValueType valueType,
        string? textValue,
        decimal? numericValue,
        bool? booleanValue,
        CompendiumEntityId? referenceId,
        string? enumValue)
    {
        ValueType = valueType;
        TextValue = Normalize(textValue);
        NumericValue = numericValue;
        BooleanValue = booleanValue;
        ReferenceId = referenceId;
        EnumValue = Normalize(enumValue);
    }

    public EffectValueType ValueType { get; private set; }
    public string? TextValue { get; private set; }
    public decimal? NumericValue { get; private set; }
    public bool? BooleanValue { get; private set; }
    public CompendiumEntityId? ReferenceId { get; private set; }
    public string? EnumValue { get; private set; }

    public static Result<TypedMechanicalValue> Create(
        EffectValueType valueType,
        string? textValue,
        decimal? numericValue,
        bool? booleanValue,
        CompendiumEntityId? referenceId,
        string? enumValue,
        string owner)
    {
        if (!Enum.IsDefined(valueType))
        {
            return Result<TypedMechanicalValue>.Failure(FeatureDomainErrors.InvalidEnum("effect-value-type"));
        }

        var value = new TypedMechanicalValue(valueType, textValue, numericValue, booleanValue, referenceId, enumValue);
        return value.HasValueForType()
            ? Result<TypedMechanicalValue>.Success(value)
            : Result<TypedMechanicalValue>.Failure(FeatureDomainErrors.TypedValueRequired(owner));
    }

    public bool Matches(EffectValueType valueType) => ValueType == valueType && HasValueForType();

    private bool HasValueForType() =>
        ValueType switch
        {
            EffectValueType.Text => !string.IsNullOrWhiteSpace(TextValue),
            EffectValueType.Number => NumericValue.HasValue,
            EffectValueType.Boolean => BooleanValue.HasValue,
            EffectValueType.Reference => ReferenceId is not null,
            EffectValueType.Enum => !string.IsNullOrWhiteSpace(EnumValue),
            _ => false
        };

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
