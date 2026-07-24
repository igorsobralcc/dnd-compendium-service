using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Features;

public sealed class ChoiceSet
{
    private readonly List<ChoiceSetFilter> filters = [];
    private readonly List<ChoiceOption> options = [];

    private ChoiceSet()
    {
        SourceEntityId = null!;
        Code = null!;
    }

    private ChoiceSet(CompendiumEntityId id, CompendiumEntityKind sourceEntityKind, CompendiumEntityId sourceEntityId, ChoiceSetCode code, int minimumChoices, int maximumChoices)
    {
        Id = id;
        SourceEntityKind = sourceEntityKind;
        SourceEntityId = sourceEntityId;
        Code = code;
        MinimumChoices = minimumChoices;
        MaximumChoices = maximumChoices;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityKind SourceEntityKind { get; private set; }
    public CompendiumEntityId SourceEntityId { get; private set; }
    public ChoiceSetCode Code { get; private set; }
    public int MinimumChoices { get; private set; }
    public int MaximumChoices { get; private set; }
    public IReadOnlyCollection<ChoiceSetFilter> Filters => filters;
    public IReadOnlyCollection<ChoiceOption> Options => options;

    public static Result<ChoiceSet> Create(CompendiumEntityKind sourceEntityKind, CompendiumEntityId sourceEntityId, ChoiceSetCode code, int minimumChoices, int maximumChoices)
    {
        if (!Enum.IsDefined(sourceEntityKind)) return Result<ChoiceSet>.Failure(FeatureDomainErrors.InvalidEnum("source-entity-kind"));
        if (minimumChoices < 0 || maximumChoices < 0 || minimumChoices > maximumChoices) return Result<ChoiceSet>.Failure(FeatureDomainErrors.ChoiceCardinalityInvalid());

        return Result<ChoiceSet>.Success(new ChoiceSet(CompendiumEntityId.New(), sourceEntityKind, sourceEntityId, code, minimumChoices, maximumChoices));
    }

    public Result AddOption(ChoiceOptionType type, CompendiumEntityId? referenceId, string? displayText, int sortOrder)
    {
        var option = ChoiceOption.Create(Id, type, referenceId, displayText, sortOrder);
        if (option.IsFailure) return Result.Failure(option.Error);
        options.Add(option.Value);
        return Result.Success();
    }

    public Result AddFilter(ChoiceFilterType type, EffectValueType valueType, string? textValue, decimal? numericValue, bool? booleanValue, CompendiumEntityId? referenceId, string? enumValue)
    {
        var filter = ChoiceSetFilter.Create(Id, type, valueType, textValue, numericValue, booleanValue, referenceId, enumValue);
        if (filter.IsFailure) return Result.Failure(filter.Error);
        filters.Add(filter.Value);
        return Result.Success();
    }
}

public sealed class ChoiceOption
{
    private ChoiceOption()
    {
        ChoiceSetId = null!;
    }

    private ChoiceOption(CompendiumEntityId id, CompendiumEntityId choiceSetId, ChoiceOptionType type, CompendiumEntityId? referenceId, string? displayText, int sortOrder)
    {
        Id = id;
        ChoiceSetId = choiceSetId;
        Type = type;
        ReferenceId = referenceId;
        DisplayText = string.IsNullOrWhiteSpace(displayText) ? null : displayText.Trim();
        SortOrder = sortOrder;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId ChoiceSetId { get; private set; }
    public ChoiceOptionType Type { get; private set; }
    public CompendiumEntityId? ReferenceId { get; private set; }
    public string? DisplayText { get; private set; }
    public int SortOrder { get; private set; }

    public static Result<ChoiceOption> Create(CompendiumEntityId choiceSetId, ChoiceOptionType type, CompendiumEntityId? referenceId, string? displayText, int sortOrder) =>
        Enum.IsDefined(type)
            ? Result<ChoiceOption>.Success(new ChoiceOption(CompendiumEntityId.New(), choiceSetId, type, referenceId, displayText, sortOrder))
            : Result<ChoiceOption>.Failure(FeatureDomainErrors.InvalidEnum("choice-option-type"));
}

public sealed class ChoiceSetFilter
{
    private ChoiceSetFilter()
    {
        ChoiceSetId = null!;
        Value = null!;
    }

    private ChoiceSetFilter(CompendiumEntityId id, CompendiumEntityId choiceSetId, ChoiceFilterType type, TypedMechanicalValue value)
    {
        Id = id;
        ChoiceSetId = choiceSetId;
        Type = type;
        Value = value;
    }

    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId ChoiceSetId { get; private set; }
    public ChoiceFilterType Type { get; private set; }
    public TypedMechanicalValue Value { get; private set; }

    public static Result<ChoiceSetFilter> Create(CompendiumEntityId choiceSetId, ChoiceFilterType type, EffectValueType valueType, string? textValue, decimal? numericValue, bool? booleanValue, CompendiumEntityId? referenceId, string? enumValue)
    {
        if (!Enum.IsDefined(type)) return Result<ChoiceSetFilter>.Failure(FeatureDomainErrors.InvalidEnum("choice-filter-type"));

        var value = TypedMechanicalValue.Create(valueType, textValue, numericValue, booleanValue, referenceId, enumValue, "choice-filter");
        return value.IsFailure
            ? Result<ChoiceSetFilter>.Failure(value.Error)
            : Result<ChoiceSetFilter>.Success(new ChoiceSetFilter(CompendiumEntityId.New(), choiceSetId, type, value.Value));
    }
}
