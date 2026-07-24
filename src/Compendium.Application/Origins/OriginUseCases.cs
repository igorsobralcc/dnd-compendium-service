using Compendium.Application.Errors;
using Compendium.Application.Features;
using Compendium.Application.Fundamentals;
using Compendium.Application.Sources;
using Compendium.Domain.Features;
using Compendium.Domain.Fundamentals;
using Compendium.Domain.Origins;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Origins;

public sealed class CreateSpeciesUseCase(
    IRuleSourceRepository sources, ISourceVersionRepository versions, ISpeciesRepository species, IClock clock)
{
    public async Task<ApplicationResult<SpeciesDetailsDto>> ExecuteAsync(CreateSpeciesCommand command, CancellationToken cancellationToken)
    {
        var code = SpeciesCode.Create(command.Code);
        var name = SpeciesName.Create(command.Name);
        var description = SpeciesDescription.CreateOptional(command.Description);
        if (code.IsFailure) return ApplicationResult<SpeciesDetailsDto>.Failure(OriginErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<SpeciesDetailsDto>.Failure(OriginErrors.FromDomain(name.Error));
        if (description.IsFailure) return ApplicationResult<SpeciesDetailsDto>.Failure(OriginErrors.FromDomain(description.Error));
        var source = await OriginValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<SpeciesDetailsDto>.Failure(source.Error);
        if (await species.ExistsByCodeAsync(code.Value, cancellationToken))
            return ApplicationResult<SpeciesDetailsDto>.Failure(OriginErrors.CodeAlreadyExists("species", code.Value.Value));

        var entity = Species.Create(source.Value.RuleSourceId, source.Value.SourceVersionId, code.Value, name.Value, description.Value, clock.UtcNow).Value;
        await species.AddAsync(entity, cancellationToken);
        await species.SaveChangesAsync(cancellationToken);
        return ApplicationResult<SpeciesDetailsDto>.Success(entity.ToDetailsDto([]));
    }
}

public sealed class LinkSpeciesFeatureUseCase(
    IRuleSourceRepository sources, ISourceVersionRepository versions, ISpeciesRepository species, IFeatureRepository features, IClock clock)
{
    public async Task<ApplicationResult<SpeciesDetailsDto>> ExecuteAsync(LinkSpeciesFeatureCommand command, CancellationToken cancellationToken)
    {
        var code = SpeciesCode.Create(command.SpeciesCode);
        var featureId = CompendiumEntityId.Create(command.FeatureId);
        if (code.IsFailure) return ApplicationResult<SpeciesDetailsDto>.Failure(OriginErrors.FromDomain(code.Error));
        if (featureId.IsFailure) return ApplicationResult<SpeciesDetailsDto>.Failure(OriginErrors.FromDomain(featureId.Error));
        var source = await OriginValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<SpeciesDetailsDto>.Failure(source.Error);
        var entity = await species.GetByCodeAsync(code.Value, cancellationToken);
        if (entity is null) return ApplicationResult<SpeciesDetailsDto>.Failure(OriginErrors.NotFound("species", code.Value.Value));
        if (!await features.ExistsByIdAsync(featureId.Value, cancellationToken))
            return ApplicationResult<SpeciesDetailsDto>.Failure(OriginErrors.ReferenceNotFound("feature", featureId.Value.ToString()));
        var link = entity.LinkFeature(featureId.Value, source.Value.SourceVersionId, clock.UtcNow);
        if (link.IsFailure) return ApplicationResult<SpeciesDetailsDto>.Failure(OriginErrors.FromDomain(link.Error));
        await species.SaveChangesAsync(cancellationToken);
        return ApplicationResult<SpeciesDetailsDto>.Success(entity.ToDetailsDto([]));
    }
}

public sealed class ListSpeciesQuery(ISpeciesRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyCollection<SpeciesSummaryDto>>> ExecuteAsync(CancellationToken cancellationToken) =>
        ApplicationResult<IReadOnlyCollection<SpeciesSummaryDto>>.Success(
            (await repository.ListAsync(cancellationToken)).Select(entity => entity.ToSummaryDto()).ToArray());
}

public sealed class GetSpeciesDetailsQuery(ISpeciesRepository species, IChoiceSetRepository choices)
{
    public async Task<ApplicationResult<SpeciesDetailsDto>> ExecuteAsync(string codeValue, CancellationToken cancellationToken)
    {
        var code = SpeciesCode.Create(codeValue);
        if (code.IsFailure) return ApplicationResult<SpeciesDetailsDto>.Failure(OriginErrors.FromDomain(code.Error));
        var entity = await species.GetByCodeAsync(code.Value, cancellationToken);
        if (entity is null) return ApplicationResult<SpeciesDetailsDto>.Failure(OriginErrors.NotFound("species", code.Value.Value));
        var entityChoices = await choices.ListBySourceEntityAsync(CompendiumEntityKind.Species, entity.Id, cancellationToken);
        return ApplicationResult<SpeciesDetailsDto>.Success(entity.ToDetailsDto(entityChoices));
    }
}

public sealed class CreateBackgroundUseCase(
    IRuleSourceRepository sources, ISourceVersionRepository versions, IBackgroundRepository backgrounds, IClock clock)
{
    public async Task<ApplicationResult<BackgroundDetailsDto>> ExecuteAsync(CreateBackgroundCommand command, CancellationToken cancellationToken)
    {
        var code = BackgroundCode.Create(command.Code);
        var name = BackgroundName.Create(command.Name);
        var description = BackgroundDescription.CreateOptional(command.Description);
        if (code.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.FromDomain(name.Error));
        if (description.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.FromDomain(description.Error));
        var source = await OriginValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(source.Error);
        if (await backgrounds.ExistsByCodeAsync(code.Value, cancellationToken))
            return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.CodeAlreadyExists("background", code.Value.Value));
        var entity = Background.Create(source.Value.RuleSourceId, source.Value.SourceVersionId, code.Value, name.Value, description.Value, clock.UtcNow).Value;
        await backgrounds.AddAsync(entity, cancellationToken);
        await backgrounds.SaveChangesAsync(cancellationToken);
        return ApplicationResult<BackgroundDetailsDto>.Success(entity.ToDetailsDto([]));
    }
}

public sealed class ConfigureBackgroundMechanicsUseCase(
    IRuleSourceRepository sources,
    ISourceVersionRepository versions,
    IBackgroundRepository backgrounds,
    IAbilityRepository abilities,
    IProficiencyRepository proficiencies,
    IFeatRepository feats,
    IClock clock)
{
    public async Task<ApplicationResult<BackgroundDetailsDto>> ExecuteAsync(ConfigureBackgroundMechanicsCommand command, CancellationToken cancellationToken)
    {
        var code = BackgroundCode.Create(command.BackgroundCode);
        if (code.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.FromDomain(code.Error));
        var source = await OriginValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(source.Error);
        var entity = await backgrounds.GetByCodeAsync(code.Value, cancellationToken);
        if (entity is null) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.NotFound("background", code.Value.Value));

        var abilityIds = OriginValidation.ParseIds(command.AbilityOptionIds);
        var featIds = OriginValidation.ParseIds(command.FeatIds);
        var skillIds = OriginValidation.ParseIds(command.SkillProficiencyIds);
        var toolIds = OriginValidation.ParseIds(command.ToolProficiencyIds);
        var equipmentIds = OriginValidation.ParseIds(command.StartingEquipmentRules.Select(rule => rule.ReferenceId).ToArray());
        foreach (var parsed in new[] { abilityIds, featIds, skillIds, toolIds, equipmentIds })
            if (parsed.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.FromDomain(parsed.Error));

        foreach (var id in abilityIds.Value)
            if (await abilities.GetByIdAsync(id, cancellationToken) is null)
                return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.ReferenceNotFound("ability", id.ToString()));
        foreach (var id in featIds.Value)
            if (await feats.GetByIdAsync(id, cancellationToken) is null)
                return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.ReferenceNotFound("feat", id.ToString()));

        var skillSet = (await proficiencies.ListAsync(ProficiencyType.Skill, cancellationToken)).Select(item => item.Id).ToHashSet();
        var toolSet = (await proficiencies.ListAsync(ProficiencyType.Tool, cancellationToken)).Select(item => item.Id).ToHashSet();
        foreach (var id in skillIds.Value)
            if (!skillSet.Contains(id)) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.ReferenceNotFound("skill-proficiency", id.ToString()));
        foreach (var id in toolIds.Value)
            if (!toolSet.Contains(id)) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.ReferenceNotFound("tool-proficiency", id.ToString()));

        var mechanics = new BackgroundMechanicsInput(
            abilityIds.Value,
            command.AbilityBoostRules.Select(rule => new BackgroundAbilityBoostRuleInput(rule.BoostAmount, rule.AbilityCount)).ToArray(),
            featIds.Value,
            skillIds.Value,
            toolIds.Value,
            command.StartingEquipmentRules.Zip(equipmentIds.Value, (rule, id) => new BackgroundStartingEquipmentRuleInput(id, rule.ReferenceType)).ToArray());
        var configure = entity.ConfigureMechanics(mechanics, source.Value.SourceVersionId, clock.UtcNow);
        if (configure.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.FromDomain(configure.Error));
        await backgrounds.SaveChangesAsync(cancellationToken);
        return ApplicationResult<BackgroundDetailsDto>.Success(entity.ToDetailsDto([]));
    }
}

public sealed class LinkBackgroundFeatureUseCase(
    IRuleSourceRepository sources, ISourceVersionRepository versions, IBackgroundRepository backgrounds, IFeatureRepository features, IClock clock)
{
    public async Task<ApplicationResult<BackgroundDetailsDto>> ExecuteAsync(LinkBackgroundFeatureCommand command, CancellationToken cancellationToken)
    {
        var code = BackgroundCode.Create(command.BackgroundCode);
        var featureId = CompendiumEntityId.Create(command.FeatureId);
        if (code.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.FromDomain(code.Error));
        if (featureId.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.FromDomain(featureId.Error));
        var source = await OriginValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(source.Error);
        var entity = await backgrounds.GetByCodeAsync(code.Value, cancellationToken);
        if (entity is null) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.NotFound("background", code.Value.Value));
        if (!await features.ExistsByIdAsync(featureId.Value, cancellationToken))
            return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.ReferenceNotFound("feature", featureId.Value.ToString()));
        var result = entity.LinkFeature(featureId.Value, source.Value.SourceVersionId, clock.UtcNow);
        if (result.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.FromDomain(result.Error));
        await backgrounds.SaveChangesAsync(cancellationToken);
        return ApplicationResult<BackgroundDetailsDto>.Success(entity.ToDetailsDto([]));
    }
}

public sealed class ListBackgroundsQuery(IBackgroundRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyCollection<BackgroundSummaryDto>>> ExecuteAsync(CancellationToken cancellationToken) =>
        ApplicationResult<IReadOnlyCollection<BackgroundSummaryDto>>.Success(
            (await repository.ListAsync(cancellationToken)).Select(entity => entity.ToSummaryDto()).ToArray());
}

public sealed class GetBackgroundDetailsQuery(IBackgroundRepository backgrounds, IChoiceSetRepository choices)
{
    public async Task<ApplicationResult<BackgroundDetailsDto>> ExecuteAsync(string codeValue, CancellationToken cancellationToken)
    {
        var code = BackgroundCode.Create(codeValue);
        if (code.IsFailure) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.FromDomain(code.Error));
        var entity = await backgrounds.GetByCodeAsync(code.Value, cancellationToken);
        if (entity is null) return ApplicationResult<BackgroundDetailsDto>.Failure(OriginErrors.NotFound("background", code.Value.Value));
        var entityChoices = await choices.ListBySourceEntityAsync(CompendiumEntityKind.Background, entity.Id, cancellationToken);
        return ApplicationResult<BackgroundDetailsDto>.Success(entity.ToDetailsDto(entityChoices));
    }
}

public sealed class CreateFeatUseCase(
    IRuleSourceRepository sources, ISourceVersionRepository versions, IFeatRepository feats, IClock clock)
{
    public async Task<ApplicationResult<FeatDetailsDto>> ExecuteAsync(CreateFeatCommand command, CancellationToken cancellationToken)
    {
        var code = FeatCode.Create(command.Code);
        var name = FeatName.Create(command.Name);
        var description = FeatDescription.CreateOptional(command.Description);
        if (code.IsFailure) return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.FromDomain(name.Error));
        if (description.IsFailure) return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.FromDomain(description.Error));
        if (!Enum.IsDefined(command.Category))
            return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.FromDomain(OriginDomainErrors.InvalidEnum("feat-category")));
        var source = await OriginValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<FeatDetailsDto>.Failure(source.Error);
        if (await feats.ExistsByCodeAsync(code.Value, cancellationToken))
            return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.CodeAlreadyExists("feat", code.Value.Value));
        var result = Feat.Create(source.Value.RuleSourceId, source.Value.SourceVersionId, code.Value, name.Value, description.Value, command.Category, command.Repeatable, clock.UtcNow);
        if (result.IsFailure) return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.FromDomain(result.Error));
        await feats.AddAsync(result.Value, cancellationToken);
        await feats.SaveChangesAsync(cancellationToken);
        return ApplicationResult<FeatDetailsDto>.Success(result.Value.ToDetailsDto([], []));
    }
}

public sealed class LinkFeatFeatureUseCase(
    IRuleSourceRepository sources, ISourceVersionRepository versions, IFeatRepository feats, IFeatureRepository features, IClock clock)
{
    public async Task<ApplicationResult<FeatDetailsDto>> ExecuteAsync(LinkFeatFeatureCommand command, CancellationToken cancellationToken)
    {
        var code = FeatCode.Create(command.FeatCode);
        var featureId = CompendiumEntityId.Create(command.FeatureId);
        if (code.IsFailure) return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.FromDomain(code.Error));
        if (featureId.IsFailure) return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.FromDomain(featureId.Error));
        var source = await OriginValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<FeatDetailsDto>.Failure(source.Error);
        var entity = await feats.GetByCodeAsync(code.Value, cancellationToken);
        if (entity is null) return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.NotFound("feat", code.Value.Value));
        if (!await features.ExistsByIdAsync(featureId.Value, cancellationToken))
            return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.ReferenceNotFound("feature", featureId.Value.ToString()));
        var result = entity.LinkFeature(featureId.Value, source.Value.SourceVersionId, clock.UtcNow);
        if (result.IsFailure) return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.FromDomain(result.Error));
        await feats.SaveChangesAsync(cancellationToken);
        return ApplicationResult<FeatDetailsDto>.Success(entity.ToDetailsDto([], []));
    }
}

public sealed class AddFeatPrerequisiteUseCase(IFeatRepository feats, IEntityPrerequisiteRepository prerequisites)
{
    public async Task<ApplicationResult<EntityPrerequisiteDto>> ExecuteAsync(AddFeatPrerequisiteCommand command, CancellationToken cancellationToken)
    {
        var code = FeatCode.Create(command.FeatCode);
        if (code.IsFailure) return ApplicationResult<EntityPrerequisiteDto>.Failure(OriginErrors.FromDomain(code.Error));
        var feat = await feats.GetByCodeAsync(code.Value, cancellationToken);
        if (feat is null) return ApplicationResult<EntityPrerequisiteDto>.Failure(OriginErrors.NotFound("feat", code.Value.Value));
        CompendiumEntityId? referenceId = null;
        if (command.ReferenceId.HasValue)
        {
            var parsed = CompendiumEntityId.Create(command.ReferenceId.Value);
            if (parsed.IsFailure) return ApplicationResult<EntityPrerequisiteDto>.Failure(OriginErrors.FromDomain(parsed.Error));
            referenceId = parsed.Value;
        }
        var result = EntityPrerequisite.Create(CompendiumEntityKind.Feat, feat.Id, command.Type, command.Operator, command.Target,
            command.ValueType, command.TextValue, command.NumericValue, command.BooleanValue, referenceId, command.EnumValue);
        if (result.IsFailure) return ApplicationResult<EntityPrerequisiteDto>.Failure(OriginErrors.FromDomain(result.Error));
        await prerequisites.AddAsync(result.Value, cancellationToken);
        await prerequisites.SaveChangesAsync(cancellationToken);
        return ApplicationResult<EntityPrerequisiteDto>.Success(result.Value.ToDto());
    }
}

public sealed class ListFeatsQuery(IFeatRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyCollection<FeatSummaryDto>>> ExecuteAsync(CancellationToken cancellationToken) =>
        ApplicationResult<IReadOnlyCollection<FeatSummaryDto>>.Success(
            (await repository.ListAsync(cancellationToken)).Select(entity => entity.ToSummaryDto()).ToArray());
}

public sealed class GetFeatDetailsQuery(IFeatRepository feats, IEntityPrerequisiteRepository prerequisites, IChoiceSetRepository choices)
{
    public async Task<ApplicationResult<FeatDetailsDto>> ExecuteAsync(string codeValue, CancellationToken cancellationToken)
    {
        var code = FeatCode.Create(codeValue);
        if (code.IsFailure) return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.FromDomain(code.Error));
        var entity = await feats.GetByCodeAsync(code.Value, cancellationToken);
        if (entity is null) return ApplicationResult<FeatDetailsDto>.Failure(OriginErrors.NotFound("feat", code.Value.Value));
        var entityPrerequisites = await prerequisites.ListByEntityAsync(CompendiumEntityKind.Feat, entity.Id, cancellationToken);
        var entityChoices = await choices.ListBySourceEntityAsync(CompendiumEntityKind.Feat, entity.Id, cancellationToken);
        return ApplicationResult<FeatDetailsDto>.Success(entity.ToDetailsDto(entityPrerequisites, entityChoices));
    }
}

file static class OriginValidation
{
    public static async Task<ApplicationResult<(CompendiumEntityId RuleSourceId, CompendiumEntityId SourceVersionId)>> ValidateSourceAsync(
        IRuleSourceRepository sources, ISourceVersionRepository versions, Guid sourceValue, Guid versionValue, CancellationToken cancellationToken)
    {
        var sourceId = CompendiumEntityId.Create(sourceValue);
        var versionId = CompendiumEntityId.Create(versionValue);
        if (sourceId.IsFailure) return ApplicationResult<(CompendiumEntityId, CompendiumEntityId)>.Failure(OriginErrors.FromDomain(sourceId.Error));
        if (versionId.IsFailure) return ApplicationResult<(CompendiumEntityId, CompendiumEntityId)>.Failure(OriginErrors.FromDomain(versionId.Error));
        var source = await sources.GetByIdAsync(sourceId.Value, cancellationToken);
        if (source is null) return ApplicationResult<(CompendiumEntityId, CompendiumEntityId)>.Failure(OriginErrors.ReferenceNotFound("rule-source", sourceId.Value.ToString()));
        var version = await versions.GetByIdAsync(versionId.Value, cancellationToken);
        if (version is null) return ApplicationResult<(CompendiumEntityId, CompendiumEntityId)>.Failure(OriginErrors.ReferenceNotFound("source-version", versionId.Value.ToString()));
        if (version.RuleSourceId != source.Id) return ApplicationResult<(CompendiumEntityId, CompendiumEntityId)>.Failure(OriginErrors.SourceVersionMismatch());
        return ApplicationResult<(CompendiumEntityId, CompendiumEntityId)>.Success((source.Id, version.Id));
    }

    public static Result<IReadOnlyCollection<CompendiumEntityId>> ParseIds(IReadOnlyCollection<Guid> values)
    {
        var ids = new List<CompendiumEntityId>(values.Count);
        foreach (var value in values)
        {
            var id = CompendiumEntityId.Create(value);
            if (id.IsFailure) return Result<IReadOnlyCollection<CompendiumEntityId>>.Failure(id.Error);
            ids.Add(id.Value);
        }
        return Result<IReadOnlyCollection<CompendiumEntityId>>.Success(ids);
    }
}

file static class OriginMappings
{
    public static SpeciesSummaryDto ToSummaryDto(this Species entity) =>
        new(entity.Id.Value, entity.RuleSourceId.Value, entity.SourceVersionId.Value, entity.Code.Value, entity.Name.Value, entity.Description?.Value);
    public static SpeciesDetailsDto ToDetailsDto(this Species entity, IReadOnlyCollection<ChoiceSet> choices) =>
        new(entity.Id.Value, entity.RuleSourceId.Value, entity.SourceVersionId.Value, entity.Code.Value, entity.Name.Value, entity.Description?.Value,
            entity.Features.Select(feature => new OriginFeatureDto(feature.Id.Value, feature.FeatureId.Value, feature.SourceVersionId.Value)).ToArray(),
            choices.Select(choice => choice.ToDto()).ToArray());
    public static BackgroundSummaryDto ToSummaryDto(this Background entity) =>
        new(entity.Id.Value, entity.RuleSourceId.Value, entity.SourceVersionId.Value, entity.Code.Value, entity.Name.Value, entity.Description?.Value);
    public static BackgroundDetailsDto ToDetailsDto(this Background entity, IReadOnlyCollection<ChoiceSet> choices) =>
        new(entity.Id.Value, entity.RuleSourceId.Value, entity.SourceVersionId.Value, entity.Code.Value, entity.Name.Value, entity.Description?.Value,
            entity.AbilityOptions.Select(item => new BackgroundAbilityOptionDto(item.Id.Value, item.AbilityId.Value, item.SortOrder)).ToArray(),
            entity.AbilityBoostRules.Select(item => new BackgroundAbilityBoostRuleDto(item.Id.Value, item.BoostAmount, item.AbilityCount)).ToArray(),
            entity.FeatGrants.Select(item => new BackgroundFeatGrantDto(item.Id.Value, item.FeatId.Value)).ToArray(),
            entity.SkillProficiencies.Select(item => new BackgroundProficiencyDto(item.Id.Value, item.ProficiencyId.Value)).ToArray(),
            entity.ToolProficiencies.Select(item => new BackgroundProficiencyDto(item.Id.Value, item.ProficiencyId.Value)).ToArray(),
            entity.StartingEquipmentRules.Select(item => new BackgroundStartingEquipmentRuleDto(item.Id.Value, item.ReferenceId.Value, item.ReferenceType)).ToArray(),
            entity.Features.Select(feature => new OriginFeatureDto(feature.Id.Value, feature.FeatureId.Value, feature.SourceVersionId.Value)).ToArray(),
            choices.Select(choice => choice.ToDto()).ToArray());
    public static FeatSummaryDto ToSummaryDto(this Feat entity) =>
        new(entity.Id.Value, entity.RuleSourceId.Value, entity.SourceVersionId.Value, entity.Code.Value, entity.Name.Value, entity.Description?.Value, entity.Category, entity.Repeatable);
    public static FeatDetailsDto ToDetailsDto(this Feat entity, IReadOnlyCollection<EntityPrerequisite> prerequisites, IReadOnlyCollection<ChoiceSet> choices) =>
        new(entity.Id.Value, entity.RuleSourceId.Value, entity.SourceVersionId.Value, entity.Code.Value, entity.Name.Value, entity.Description?.Value, entity.Category, entity.Repeatable,
            entity.Features.Select(feature => new OriginFeatureDto(feature.Id.Value, feature.FeatureId.Value, feature.SourceVersionId.Value)).ToArray(),
            prerequisites.Select(item => item.ToDto()).ToArray(), choices.Select(choice => choice.ToDto()).ToArray());
    public static EntityPrerequisiteDto ToDto(this EntityPrerequisite item) =>
        new(item.Id.Value, item.EntityKind, item.EntityId.Value, item.Type, item.Operator, item.Target, item.Value.ToDto());
    public static ChoiceSetDto ToDto(this ChoiceSet item) =>
        new(item.Id.Value, item.SourceEntityKind, item.SourceEntityId.Value, item.Code.Value, item.MinimumChoices, item.MaximumChoices,
            item.Filters.Select(filter => new ChoiceSetFilterDto(filter.Id.Value, filter.Type, filter.Value.ToDto())).ToArray(),
            item.Options.Select(option => new ChoiceOptionDto(option.Id.Value, option.Type, option.ReferenceId?.Value, option.DisplayText, option.SortOrder)).ToArray());
    private static TypedMechanicalValueDto ToDto(this TypedMechanicalValue value) =>
        new(value.ValueType, value.TextValue, value.NumericValue, value.BooleanValue, value.ReferenceId?.Value, value.EnumValue);
}
