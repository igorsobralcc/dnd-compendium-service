using Compendium.Application.Errors;
using Compendium.Application.Fundamentals;
using Compendium.Application.Sources;
using Compendium.Domain.Features;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Features;

public sealed class CreateFeatureUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IFeatureRepository features;
    private readonly IClock clock;

    public CreateFeatureUseCase(IRuleSourceRepository sources, ISourceVersionRepository versions, IFeatureRepository features, IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.features = features;
        this.clock = clock;
    }

    public async Task<ApplicationResult<FeatureDetailsDto>> ExecuteAsync(CreateFeatureCommand command, CancellationToken cancellationToken)
    {
        var code = FeatureCode.Create(command.Code);
        var name = FeatureName.Create(command.Name);
        var description = FeatureDescription.CreateOptional(command.Description);
        if (code.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(name.Error));
        if (description.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(description.Error));

        var source = await FeatureReferenceValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(source.Error);

        if (await features.ExistsByCodeAsync(code.Value, cancellationToken))
        {
            return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FeatureCodeAlreadyExists(code.Value.Value));
        }

        var feature = Feature.Create(source.Value.RuleSourceId, source.Value.SourceVersionId, code.Value, name.Value, description.Value, command.LevelRequirement, clock.UtcNow);
        if (feature.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(feature.Error));

        await features.AddAsync(feature.Value, cancellationToken);
        await features.SaveChangesAsync(cancellationToken);
        return ApplicationResult<FeatureDetailsDto>.Success(feature.Value.ToDetailsDto());
    }
}

public sealed class UpdateFeatureUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IFeatureRepository features;
    private readonly IClock clock;

    public UpdateFeatureUseCase(IRuleSourceRepository sources, ISourceVersionRepository versions, IFeatureRepository features, IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.features = features;
        this.clock = clock;
    }

    public async Task<ApplicationResult<FeatureDetailsDto>> ExecuteAsync(UpdateFeatureCommand command, CancellationToken cancellationToken)
    {
        var code = FeatureCode.Create(command.Code);
        var name = FeatureName.Create(command.Name);
        var description = FeatureDescription.CreateOptional(command.Description);
        if (code.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(name.Error));
        if (description.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(description.Error));

        var source = await FeatureReferenceValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(source.Error);

        var feature = await features.GetByCodeAsync(code.Value, cancellationToken);
        if (feature is null) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FeatureNotFound(code.Value.Value));

        var update = feature.Update(source.Value.SourceVersionId, name.Value, description.Value, command.LevelRequirement, clock.UtcNow);
        if (update.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(update.Error));

        await features.SaveChangesAsync(cancellationToken);
        return ApplicationResult<FeatureDetailsDto>.Success(feature.ToDetailsDto());
    }
}

public sealed class ListFeaturesQuery
{
    private readonly IFeatureRepository features;
    public ListFeaturesQuery(IFeatureRepository features) => this.features = features;

    public async Task<ApplicationResult<IReadOnlyCollection<FeatureSummaryDto>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var result = await features.ListAsync(cancellationToken);
        return ApplicationResult<IReadOnlyCollection<FeatureSummaryDto>>.Success(result.Select(feature => feature.ToSummaryDto()).ToArray());
    }
}

public sealed class GetFeatureDetailsQuery
{
    private readonly IFeatureRepository features;
    public GetFeatureDetailsQuery(IFeatureRepository features) => this.features = features;

    public async Task<ApplicationResult<FeatureDetailsDto>> ExecuteAsync(string codeValue, CancellationToken cancellationToken)
    {
        var code = FeatureCode.Create(codeValue);
        if (code.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(code.Error));

        var feature = await features.GetByCodeAsync(code.Value, cancellationToken);
        return feature is null
            ? ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FeatureNotFound(code.Value.Value))
            : ApplicationResult<FeatureDetailsDto>.Success(feature.ToDetailsDto());
    }
}

public sealed class CreateEffectSchemaUseCase
{
    private readonly IEffectSchemaRepository schemas;
    public CreateEffectSchemaUseCase(IEffectSchemaRepository schemas) => this.schemas = schemas;

    public async Task<ApplicationResult<EffectSchemaDto>> ExecuteAsync(CreateEffectSchemaCommand command, CancellationToken cancellationToken)
    {
        var code = FeatureCode.Create(command.Code);
        var name = FeatureName.Create(command.Name);
        if (code.IsFailure) return ApplicationResult<EffectSchemaDto>.Failure(FeatureErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<EffectSchemaDto>.Failure(FeatureErrors.FromDomain(name.Error));

        if (await schemas.GetByCodeAsync(code.Value, cancellationToken) is not null)
        {
            return ApplicationResult<EffectSchemaDto>.Failure(FeatureErrors.EffectSchemaCodeAlreadyExists(code.Value.Value));
        }

        var schema = EffectSchema.Create(code.Value, name.Value, command.Type, (command.Fields ?? []).Select(field => new EffectSchemaFieldInput(field.Code, field.ValueType, field.IsRequired)).ToArray());
        if (schema.IsFailure) return ApplicationResult<EffectSchemaDto>.Failure(FeatureErrors.FromDomain(schema.Error));

        await schemas.AddAsync(schema.Value, cancellationToken);
        await schemas.SaveChangesAsync(cancellationToken);
        return ApplicationResult<EffectSchemaDto>.Success(schema.Value.ToDto());
    }
}

public sealed class AttachEffectToFeatureUseCase
{
    private readonly IFeatureRepository features;
    private readonly IEffectSchemaRepository schemas;
    private readonly IClock clock;

    public AttachEffectToFeatureUseCase(IFeatureRepository features, IEffectSchemaRepository schemas, IClock clock)
    {
        this.features = features;
        this.schemas = schemas;
        this.clock = clock;
    }

    public async Task<ApplicationResult<FeatureDetailsDto>> ExecuteAsync(AttachEffectToFeatureCommand command, CancellationToken cancellationToken)
    {
        var featureCode = FeatureCode.Create(command.FeatureCode);
        var schemaCode = FeatureCode.Create(command.EffectSchemaCode);
        if (featureCode.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(featureCode.Error));
        if (schemaCode.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(schemaCode.Error));

        var feature = await features.GetByCodeAsync(featureCode.Value, cancellationToken);
        if (feature is null) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FeatureNotFound(featureCode.Value.Value));

        var schema = await schemas.GetByCodeAsync(schemaCode.Value, cancellationToken);
        if (schema is null) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.EffectSchemaNotFound(schemaCode.Value.Value));

        var fieldInputs = FeatureReferenceValidation.ResolveFieldInputs(command.Fields ?? []);
        if (fieldInputs.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(fieldInputs.Error);

        var conditionInputs = FeatureReferenceValidation.ResolveConditionInputs(command.Conditions ?? []);
        if (conditionInputs.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(conditionInputs.Error);

        var attach = feature.AttachEffect(schema, new FeatureEffectInput(command.Type, command.Target, fieldInputs.Value, conditionInputs.Value), clock.UtcNow);
        if (attach.IsFailure) return ApplicationResult<FeatureDetailsDto>.Failure(FeatureErrors.FromDomain(attach.Error));

        await features.SaveChangesAsync(cancellationToken);
        return ApplicationResult<FeatureDetailsDto>.Success(feature.ToDetailsDto());
    }
}

public sealed class GetFeatureEffectsQuery
{
    private readonly IFeatureRepository features;
    public GetFeatureEffectsQuery(IFeatureRepository features) => this.features = features;

    public async Task<ApplicationResult<IReadOnlyCollection<FeatureEffectDto>>> ExecuteAsync(string featureCodeValue, CancellationToken cancellationToken)
    {
        var code = FeatureCode.Create(featureCodeValue);
        if (code.IsFailure) return ApplicationResult<IReadOnlyCollection<FeatureEffectDto>>.Failure(FeatureErrors.FromDomain(code.Error));

        var feature = await features.GetByCodeAsync(code.Value, cancellationToken);
        return feature is null
            ? ApplicationResult<IReadOnlyCollection<FeatureEffectDto>>.Failure(FeatureErrors.FeatureNotFound(code.Value.Value))
            : ApplicationResult<IReadOnlyCollection<FeatureEffectDto>>.Success(feature.Effects.Select(effect => effect.ToDto()).ToArray());
    }
}

public sealed class AddPrerequisiteToEntityUseCase
{
    private readonly IEntityPrerequisiteRepository prerequisites;
    public AddPrerequisiteToEntityUseCase(IEntityPrerequisiteRepository prerequisites) => this.prerequisites = prerequisites;

    public async Task<ApplicationResult<EntityPrerequisiteDto>> ExecuteAsync(AddPrerequisiteToEntityCommand command, CancellationToken cancellationToken)
    {
        var entityId = CompendiumEntityId.Create(command.EntityId);
        if (entityId.IsFailure) return ApplicationResult<EntityPrerequisiteDto>.Failure(FeatureErrors.FromDomain(entityId.Error));

        var referenceId = FeatureReferenceValidation.ResolveOptionalId(command.ReferenceId);
        if (referenceId.IsFailure) return ApplicationResult<EntityPrerequisiteDto>.Failure(referenceId.Error);

        var prerequisite = EntityPrerequisite.Create(command.EntityKind, entityId.Value, command.Type, command.Operator, command.Target, command.ValueType, command.TextValue, command.NumericValue, command.BooleanValue, referenceId.Value, command.EnumValue);
        if (prerequisite.IsFailure) return ApplicationResult<EntityPrerequisiteDto>.Failure(FeatureErrors.FromDomain(prerequisite.Error));

        await prerequisites.AddAsync(prerequisite.Value, cancellationToken);
        await prerequisites.SaveChangesAsync(cancellationToken);
        return ApplicationResult<EntityPrerequisiteDto>.Success(prerequisite.Value.ToDto());
    }
}

public sealed class RemovePrerequisiteFromEntityUseCase
{
    private readonly IEntityPrerequisiteRepository prerequisites;
    public RemovePrerequisiteFromEntityUseCase(IEntityPrerequisiteRepository prerequisites) => this.prerequisites = prerequisites;

    public async Task<ApplicationResult> ExecuteAsync(RemovePrerequisiteFromEntityCommand command, CancellationToken cancellationToken)
    {
        var id = CompendiumEntityId.Create(command.PrerequisiteId);
        if (id.IsFailure) return ApplicationResult.Failure(FeatureErrors.FromDomain(id.Error));

        var prerequisite = await prerequisites.GetByIdAsync(id.Value, cancellationToken);
        if (prerequisite is null) return ApplicationResult.Failure(FeatureErrors.PrerequisiteNotFound(id.Value.ToString()));

        prerequisites.Remove(prerequisite);
        await prerequisites.SaveChangesAsync(cancellationToken);
        return ApplicationResult.Success();
    }
}

public sealed class GetEntityPrerequisitesQuery
{
    private readonly IEntityPrerequisiteRepository prerequisites;
    public GetEntityPrerequisitesQuery(IEntityPrerequisiteRepository prerequisites) => this.prerequisites = prerequisites;

    public async Task<ApplicationResult<IReadOnlyCollection<EntityPrerequisiteDto>>> ExecuteAsync(CompendiumEntityKind entityKind, Guid entityIdValue, CancellationToken cancellationToken)
    {
        var entityId = CompendiumEntityId.Create(entityIdValue);
        if (entityId.IsFailure) return ApplicationResult<IReadOnlyCollection<EntityPrerequisiteDto>>.Failure(FeatureErrors.FromDomain(entityId.Error));

        var result = await prerequisites.ListByEntityAsync(entityKind, entityId.Value, cancellationToken);
        return ApplicationResult<IReadOnlyCollection<EntityPrerequisiteDto>>.Success(result.Select(prerequisite => prerequisite.ToDto()).ToArray());
    }
}

public sealed class CreateChoiceSetUseCase
{
    private readonly IChoiceSetRepository choiceSets;
    public CreateChoiceSetUseCase(IChoiceSetRepository choiceSets) => this.choiceSets = choiceSets;

    public async Task<ApplicationResult<ChoiceSetDto>> ExecuteAsync(CreateChoiceSetCommand command, CancellationToken cancellationToken)
    {
        var sourceEntityId = CompendiumEntityId.Create(command.SourceEntityId);
        var code = ChoiceSetCode.Create(command.Code);
        if (sourceEntityId.IsFailure) return ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.FromDomain(sourceEntityId.Error));
        if (code.IsFailure) return ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.FromDomain(code.Error));

        if (await choiceSets.ExistsByCodeAsync(code.Value, cancellationToken))
        {
            return ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.ChoiceSetCodeAlreadyExists(code.Value.Value));
        }

        var choiceSet = ChoiceSet.Create(command.SourceEntityKind, sourceEntityId.Value, code.Value, command.MinimumChoices, command.MaximumChoices);
        if (choiceSet.IsFailure) return ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.FromDomain(choiceSet.Error));

        await choiceSets.AddAsync(choiceSet.Value, cancellationToken);
        await choiceSets.SaveChangesAsync(cancellationToken);
        return ApplicationResult<ChoiceSetDto>.Success(choiceSet.Value.ToDto());
    }
}

public sealed class AddChoiceOptionUseCase
{
    private readonly IChoiceSetRepository choiceSets;
    public AddChoiceOptionUseCase(IChoiceSetRepository choiceSets) => this.choiceSets = choiceSets;

    public async Task<ApplicationResult<ChoiceSetDto>> ExecuteAsync(AddChoiceOptionCommand command, CancellationToken cancellationToken)
    {
        var code = ChoiceSetCode.Create(command.ChoiceSetCode);
        if (code.IsFailure) return ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.FromDomain(code.Error));

        var choiceSet = await choiceSets.GetByCodeAsync(code.Value, cancellationToken);
        if (choiceSet is null) return ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.ChoiceSetNotFound(code.Value.Value));

        var referenceId = FeatureReferenceValidation.ResolveOptionalId(command.ReferenceId);
        if (referenceId.IsFailure) return ApplicationResult<ChoiceSetDto>.Failure(referenceId.Error);

        var add = choiceSet.AddOption(command.Type, referenceId.Value, command.DisplayText, command.SortOrder);
        if (add.IsFailure) return ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.FromDomain(add.Error));

        await choiceSets.SaveChangesAsync(cancellationToken);
        return ApplicationResult<ChoiceSetDto>.Success(choiceSet.ToDto());
    }
}

public sealed class AddChoiceFilterUseCase
{
    private readonly IChoiceSetRepository choiceSets;
    public AddChoiceFilterUseCase(IChoiceSetRepository choiceSets) => this.choiceSets = choiceSets;

    public async Task<ApplicationResult<ChoiceSetDto>> ExecuteAsync(AddChoiceFilterCommand command, CancellationToken cancellationToken)
    {
        var code = ChoiceSetCode.Create(command.ChoiceSetCode);
        if (code.IsFailure) return ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.FromDomain(code.Error));

        var choiceSet = await choiceSets.GetByCodeAsync(code.Value, cancellationToken);
        if (choiceSet is null) return ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.ChoiceSetNotFound(code.Value.Value));

        var referenceId = FeatureReferenceValidation.ResolveOptionalId(command.ReferenceId);
        if (referenceId.IsFailure) return ApplicationResult<ChoiceSetDto>.Failure(referenceId.Error);

        var add = choiceSet.AddFilter(command.Type, command.ValueType, command.TextValue, command.NumericValue, command.BooleanValue, referenceId.Value, command.EnumValue);
        if (add.IsFailure) return ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.FromDomain(add.Error));

        await choiceSets.SaveChangesAsync(cancellationToken);
        return ApplicationResult<ChoiceSetDto>.Success(choiceSet.ToDto());
    }
}

public sealed class GetChoiceSetDetailsQuery
{
    private readonly IChoiceSetRepository choiceSets;
    public GetChoiceSetDetailsQuery(IChoiceSetRepository choiceSets) => this.choiceSets = choiceSets;

    public async Task<ApplicationResult<ChoiceSetDto>> ExecuteAsync(string codeValue, CancellationToken cancellationToken)
    {
        var code = ChoiceSetCode.Create(codeValue);
        if (code.IsFailure) return ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.FromDomain(code.Error));

        var choiceSet = await choiceSets.GetByCodeAsync(code.Value, cancellationToken);
        return choiceSet is null
            ? ApplicationResult<ChoiceSetDto>.Failure(FeatureErrors.ChoiceSetNotFound(code.Value.Value))
            : ApplicationResult<ChoiceSetDto>.Success(choiceSet.ToDto());
    }
}

public sealed class ListChoiceSetsBySourceEntityQuery
{
    private readonly IChoiceSetRepository choiceSets;
    public ListChoiceSetsBySourceEntityQuery(IChoiceSetRepository choiceSets) => this.choiceSets = choiceSets;

    public async Task<ApplicationResult<IReadOnlyCollection<ChoiceSetDto>>> ExecuteAsync(CompendiumEntityKind entityKind, Guid entityIdValue, CancellationToken cancellationToken)
    {
        var entityId = CompendiumEntityId.Create(entityIdValue);
        if (entityId.IsFailure) return ApplicationResult<IReadOnlyCollection<ChoiceSetDto>>.Failure(FeatureErrors.FromDomain(entityId.Error));

        var result = await choiceSets.ListBySourceEntityAsync(entityKind, entityId.Value, cancellationToken);
        return ApplicationResult<IReadOnlyCollection<ChoiceSetDto>>.Success(result.Select(choiceSet => choiceSet.ToDto()).ToArray());
    }
}

internal sealed record FeatureSourceReference(CompendiumEntityId RuleSourceId, CompendiumEntityId SourceVersionId);

internal static class FeatureReferenceValidation
{
    public static async Task<ApplicationResult<FeatureSourceReference>> ValidateSourceAsync(IRuleSourceRepository sources, ISourceVersionRepository versions, Guid ruleSourceIdValue, Guid sourceVersionIdValue, CancellationToken cancellationToken)
    {
        var ruleSourceId = CompendiumEntityId.Create(ruleSourceIdValue);
        var sourceVersionId = CompendiumEntityId.Create(sourceVersionIdValue);
        if (ruleSourceId.IsFailure) return ApplicationResult<FeatureSourceReference>.Failure(FeatureErrors.FromDomain(ruleSourceId.Error));
        if (sourceVersionId.IsFailure) return ApplicationResult<FeatureSourceReference>.Failure(FeatureErrors.FromDomain(sourceVersionId.Error));
        if (await sources.GetByIdAsync(ruleSourceId.Value, cancellationToken) is null) return ApplicationResult<FeatureSourceReference>.Failure(SourceErrors.RuleSourceNotFound(ruleSourceId.Value.ToString()));

        var sourceVersion = await versions.GetByIdAsync(sourceVersionId.Value, cancellationToken);
        if (sourceVersion is null) return ApplicationResult<FeatureSourceReference>.Failure(FundamentalErrors.SourceVersionNotFound(sourceVersionId.Value.ToString()));
        if (sourceVersion.RuleSourceId != ruleSourceId.Value) return ApplicationResult<FeatureSourceReference>.Failure(FundamentalErrors.SourceVersionDoesNotBelongToSource(sourceVersionId.Value.ToString(), ruleSourceId.Value.ToString()));

        return ApplicationResult<FeatureSourceReference>.Success(new FeatureSourceReference(ruleSourceId.Value, sourceVersionId.Value));
    }

    public static ApplicationResult<CompendiumEntityId?> ResolveOptionalId(Guid? id)
    {
        if (!id.HasValue) return ApplicationResult<CompendiumEntityId?>.Success(null);
        var parsed = CompendiumEntityId.Create(id.Value);
        return parsed.IsFailure
            ? ApplicationResult<CompendiumEntityId?>.Failure(FeatureErrors.FromDomain(parsed.Error))
            : ApplicationResult<CompendiumEntityId?>.Success(parsed.Value);
    }

    public static ApplicationResult<IReadOnlyCollection<FeatureEffectFieldInput>> ResolveFieldInputs(IReadOnlyCollection<TypedValueFieldCommand> commands)
    {
        var inputs = new List<FeatureEffectFieldInput>();
        foreach (var command in commands)
        {
            var referenceId = ResolveOptionalId(command.ReferenceId);
            if (referenceId.IsFailure) return ApplicationResult<IReadOnlyCollection<FeatureEffectFieldInput>>.Failure(referenceId.Error);
            inputs.Add(new FeatureEffectFieldInput(command.FieldCode, command.TextValue, command.NumericValue, command.BooleanValue, referenceId.Value, command.EnumValue));
        }

        return ApplicationResult<IReadOnlyCollection<FeatureEffectFieldInput>>.Success(inputs);
    }

    public static ApplicationResult<IReadOnlyCollection<FeatureEffectConditionInput>> ResolveConditionInputs(IReadOnlyCollection<FeatureEffectConditionCommand> commands)
    {
        var inputs = new List<FeatureEffectConditionInput>();
        foreach (var command in commands)
        {
            var referenceId = ResolveOptionalId(command.ReferenceId);
            if (referenceId.IsFailure) return ApplicationResult<IReadOnlyCollection<FeatureEffectConditionInput>>.Failure(referenceId.Error);
            inputs.Add(new FeatureEffectConditionInput(command.Type, command.ValueType, command.TextValue, command.NumericValue, command.BooleanValue, referenceId.Value, command.EnumValue));
        }

        return ApplicationResult<IReadOnlyCollection<FeatureEffectConditionInput>>.Success(inputs);
    }
}

internal static class FeatureMapping
{
    public static FeatureSummaryDto ToSummaryDto(this Feature feature) =>
        new(feature.Id.Value, feature.RuleSourceId.Value, feature.SourceVersionId.Value, feature.Code.Value, feature.Name.Value, feature.Description?.Value, feature.LevelRequirement);

    public static FeatureDetailsDto ToDetailsDto(this Feature feature) =>
        new(feature.Id.Value, feature.RuleSourceId.Value, feature.SourceVersionId.Value, feature.Code.Value, feature.Name.Value, feature.Description?.Value, feature.LevelRequirement, feature.Effects.Select(effect => effect.ToDto()).ToArray());

    public static EffectSchemaDto ToDto(this EffectSchema schema) =>
        new(schema.Id.Value, schema.Code.Value, schema.Name.Value, schema.Type, schema.Fields.OrderBy(field => field.SortOrder).Select(field => new EffectSchemaFieldDto(field.Id.Value, field.Code.Value, field.ValueType, field.IsRequired, field.SortOrder)).ToArray());

    public static FeatureEffectDto ToDto(this FeatureEffect effect) =>
        new(effect.Id.Value, effect.EffectSchemaId.Value, effect.Type, effect.Target, effect.FieldValues.Select(value => new FeatureEffectFieldValueDto(value.Id.Value, value.EffectSchemaFieldId.Value, value.Value.ToDto())).ToArray(), effect.Conditions.Select(condition => condition.ToDto()).ToArray());

    public static FeatureEffectConditionDto ToDto(this FeatureEffectCondition condition) =>
        new(condition.Id.Value, condition.Type, condition.Value.ToDto());

    public static EntityPrerequisiteDto ToDto(this EntityPrerequisite prerequisite) =>
        new(prerequisite.Id.Value, prerequisite.EntityKind, prerequisite.EntityId.Value, prerequisite.Type, prerequisite.Operator, prerequisite.Target, prerequisite.Value.ToDto());

    public static ChoiceSetDto ToDto(this ChoiceSet choiceSet) =>
        new(choiceSet.Id.Value, choiceSet.SourceEntityKind, choiceSet.SourceEntityId.Value, choiceSet.Code.Value, choiceSet.MinimumChoices, choiceSet.MaximumChoices, choiceSet.Filters.Select(filter => filter.ToDto()).ToArray(), choiceSet.Options.OrderBy(option => option.SortOrder).Select(option => option.ToDto()).ToArray());

    public static ChoiceSetFilterDto ToDto(this ChoiceSetFilter filter) =>
        new(filter.Id.Value, filter.Type, filter.Value.ToDto());

    public static ChoiceOptionDto ToDto(this ChoiceOption option) =>
        new(option.Id.Value, option.Type, option.ReferenceId?.Value, option.DisplayText, option.SortOrder);

    public static TypedMechanicalValueDto ToDto(this TypedMechanicalValue value) =>
        new(value.ValueType, value.TextValue, value.NumericValue, value.BooleanValue, value.ReferenceId?.Value, value.EnumValue);
}
