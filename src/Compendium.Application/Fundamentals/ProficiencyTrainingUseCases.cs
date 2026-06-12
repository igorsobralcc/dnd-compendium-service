using Compendium.Application.Errors;
using Compendium.Application.Sources;
using Compendium.Domain.Fundamentals;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Fundamentals;

public sealed class CreateProficiencyUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IAbilityRepository abilities;
    private readonly ISkillRepository skills;
    private readonly ILanguageRepository languages;
    private readonly IArmorTrainingCategoryRepository armorCategories;
    private readonly IProficiencyRepository proficiencies;
    private readonly IClock clock;

    public CreateProficiencyUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IAbilityRepository abilities,
        ISkillRepository skills,
        ILanguageRepository languages,
        IArmorTrainingCategoryRepository armorCategories,
        IProficiencyRepository proficiencies,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.abilities = abilities;
        this.skills = skills;
        this.languages = languages;
        this.armorCategories = armorCategories;
        this.proficiencies = proficiencies;
        this.clock = clock;
    }

    public async Task<ApplicationResult<ProficiencyDto>> ExecuteAsync(CreateProficiencyCommand command, CancellationToken cancellationToken)
    {
        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        var code = ProficiencyCode.Create(command.Code);
        var name = DisplayName.Create(command.Name);
        var relatedEntityId = await ResolveRelatedEntityIdAsync(command.Type, command.RelatedEntityId, cancellationToken);

        if (source.IsFailure) return ApplicationResult<ProficiencyDto>.Failure(source.Error);
        if (code.IsFailure) return ApplicationResult<ProficiencyDto>.Failure(FundamentalErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<ProficiencyDto>.Failure(FundamentalErrors.FromDomain(name.Error));
        if (relatedEntityId.IsFailure) return ApplicationResult<ProficiencyDto>.Failure(relatedEntityId.Error);

        if (await proficiencies.ExistsByCodeAsync(code.Value, cancellationToken))
        {
            return ApplicationResult<ProficiencyDto>.Failure(FundamentalErrors.ProficiencyCodeAlreadyExists(code.Value.Value));
        }

        var proficiency = Proficiency.Create(
            source.Value.RuleSourceId,
            source.Value.SourceVersionId,
            code.Value,
            name.Value,
            command.Type,
            relatedEntityId.Value,
            clock.UtcNow);

        if (proficiency.IsFailure)
        {
            return ApplicationResult<ProficiencyDto>.Failure(FundamentalErrors.FromDomain(proficiency.Error));
        }

        await proficiencies.AddAsync(proficiency.Value, cancellationToken);
        await proficiencies.SaveChangesAsync(cancellationToken);
        return ApplicationResult<ProficiencyDto>.Success(proficiency.Value.ToDto());
    }

    private async Task<ApplicationResult<CompendiumEntityId?>> ResolveRelatedEntityIdAsync(
        ProficiencyType type,
        Guid? relatedEntityIdValue,
        CancellationToken cancellationToken)
    {
        if (!relatedEntityIdValue.HasValue)
        {
            return ApplicationResult<CompendiumEntityId?>.Success(null);
        }

        var relatedEntityId = CompendiumEntityId.Create(relatedEntityIdValue.Value);
        if (relatedEntityId.IsFailure)
        {
            return ApplicationResult<CompendiumEntityId?>.Failure(FundamentalErrors.FromDomain(relatedEntityId.Error));
        }

        var exists = type switch
        {
            ProficiencyType.Skill => await skills.GetByIdAsync(relatedEntityId.Value, cancellationToken) is not null,
            ProficiencyType.Language => await languages.GetByIdAsync(relatedEntityId.Value, cancellationToken) is not null,
            ProficiencyType.SavingThrow => await abilities.GetByIdAsync(relatedEntityId.Value, cancellationToken) is not null,
            ProficiencyType.Armor => await armorCategories.GetByIdAsync(relatedEntityId.Value, cancellationToken) is not null,
            _ => true
        };

        return exists
            ? ApplicationResult<CompendiumEntityId?>.Success(relatedEntityId.Value)
            : ApplicationResult<CompendiumEntityId?>.Failure(FundamentalErrors.RelatedEntityNotFound(relatedEntityId.Value.ToString()));
    }
}

public sealed class UpdateProficiencyUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IAbilityRepository abilities;
    private readonly ISkillRepository skills;
    private readonly ILanguageRepository languages;
    private readonly IArmorTrainingCategoryRepository armorCategories;
    private readonly IProficiencyRepository proficiencies;
    private readonly IClock clock;

    public UpdateProficiencyUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IAbilityRepository abilities,
        ISkillRepository skills,
        ILanguageRepository languages,
        IArmorTrainingCategoryRepository armorCategories,
        IProficiencyRepository proficiencies,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.abilities = abilities;
        this.skills = skills;
        this.languages = languages;
        this.armorCategories = armorCategories;
        this.proficiencies = proficiencies;
        this.clock = clock;
    }

    public async Task<ApplicationResult<ProficiencyDto>> ExecuteAsync(UpdateProficiencyCommand command, CancellationToken cancellationToken)
    {
        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        var code = ProficiencyCode.Create(command.Code);
        var name = DisplayName.Create(command.Name);
        var relatedEntityId = await ResolveRelatedEntityIdAsync(command.Type, command.RelatedEntityId, cancellationToken);

        if (source.IsFailure) return ApplicationResult<ProficiencyDto>.Failure(source.Error);
        if (code.IsFailure) return ApplicationResult<ProficiencyDto>.Failure(FundamentalErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<ProficiencyDto>.Failure(FundamentalErrors.FromDomain(name.Error));
        if (relatedEntityId.IsFailure) return ApplicationResult<ProficiencyDto>.Failure(relatedEntityId.Error);

        var proficiency = await proficiencies.GetByCodeAsync(code.Value, cancellationToken);
        if (proficiency is null)
        {
            return ApplicationResult<ProficiencyDto>.Failure(FundamentalErrors.ProficiencyNotFound(code.Value.Value));
        }

        var update = proficiency.Update(name.Value, source.Value.SourceVersionId, command.Type, relatedEntityId.Value, clock.UtcNow);
        if (update.IsFailure)
        {
            return ApplicationResult<ProficiencyDto>.Failure(FundamentalErrors.FromDomain(update.Error));
        }

        await proficiencies.SaveChangesAsync(cancellationToken);
        return ApplicationResult<ProficiencyDto>.Success(proficiency.ToDto());
    }

    private async Task<ApplicationResult<CompendiumEntityId?>> ResolveRelatedEntityIdAsync(
        ProficiencyType type,
        Guid? relatedEntityIdValue,
        CancellationToken cancellationToken)
    {
        if (!relatedEntityIdValue.HasValue)
        {
            return ApplicationResult<CompendiumEntityId?>.Success(null);
        }

        var relatedEntityId = CompendiumEntityId.Create(relatedEntityIdValue.Value);
        if (relatedEntityId.IsFailure)
        {
            return ApplicationResult<CompendiumEntityId?>.Failure(FundamentalErrors.FromDomain(relatedEntityId.Error));
        }

        var exists = type switch
        {
            ProficiencyType.Skill => await skills.GetByIdAsync(relatedEntityId.Value, cancellationToken) is not null,
            ProficiencyType.Language => await languages.GetByIdAsync(relatedEntityId.Value, cancellationToken) is not null,
            ProficiencyType.SavingThrow => await abilities.GetByIdAsync(relatedEntityId.Value, cancellationToken) is not null,
            ProficiencyType.Armor => await armorCategories.GetByIdAsync(relatedEntityId.Value, cancellationToken) is not null,
            _ => true
        };

        return exists
            ? ApplicationResult<CompendiumEntityId?>.Success(relatedEntityId.Value)
            : ApplicationResult<CompendiumEntityId?>.Failure(FundamentalErrors.RelatedEntityNotFound(relatedEntityId.Value.ToString()));
    }
}

public sealed class ListProficienciesQuery
{
    private readonly IProficiencyRepository repository;

    public ListProficienciesQuery(IProficiencyRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<IReadOnlyCollection<ProficiencyDto>>> ExecuteAsync(
        ProficiencyType? type,
        CancellationToken cancellationToken)
    {
        var proficiencies = await repository.ListAsync(type, cancellationToken);
        return ApplicationResult<IReadOnlyCollection<ProficiencyDto>>.Success(proficiencies.Select(proficiency => proficiency.ToDto()).ToArray());
    }
}

public sealed class CreateArmorTrainingCategoryUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IArmorTrainingCategoryRepository categories;
    private readonly IClock clock;

    public CreateArmorTrainingCategoryUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IArmorTrainingCategoryRepository categories,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.categories = categories;
        this.clock = clock;
    }

    public async Task<ApplicationResult<ArmorTrainingCategoryDto>> ExecuteAsync(
        CreateArmorTrainingCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        var code = ArmorTrainingCategoryCode.Create(command.Code);
        var name = DisplayName.Create(command.Name);

        if (source.IsFailure) return ApplicationResult<ArmorTrainingCategoryDto>.Failure(source.Error);
        if (code.IsFailure) return ApplicationResult<ArmorTrainingCategoryDto>.Failure(FundamentalErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<ArmorTrainingCategoryDto>.Failure(FundamentalErrors.FromDomain(name.Error));

        if (await categories.ExistsByCodeAsync(code.Value, cancellationToken))
        {
            return ApplicationResult<ArmorTrainingCategoryDto>.Failure(FundamentalErrors.ArmorTrainingCategoryCodeAlreadyExists(code.Value.Value));
        }

        var category = ArmorTrainingCategory.Create(
            source.Value.RuleSourceId,
            source.Value.SourceVersionId,
            code.Value,
            name.Value,
            command.SortOrder,
            clock.UtcNow);

        await categories.AddAsync(category.Value, cancellationToken);
        await categories.SaveChangesAsync(cancellationToken);
        return ApplicationResult<ArmorTrainingCategoryDto>.Success(category.Value.ToDto());
    }
}

public sealed class ListArmorTrainingCategoriesQuery
{
    private readonly IArmorTrainingCategoryRepository repository;

    public ListArmorTrainingCategoriesQuery(IArmorTrainingCategoryRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<IReadOnlyCollection<ArmorTrainingCategoryDto>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var categories = await repository.ListAsync(cancellationToken);
        return ApplicationResult<IReadOnlyCollection<ArmorTrainingCategoryDto>>.Success(categories.Select(category => category.ToDto()).ToArray());
    }
}

public sealed class CreateHitDieUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IHitDieRepository hitDice;
    private readonly IClock clock;

    public CreateHitDieUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IHitDieRepository hitDice,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.hitDice = hitDice;
        this.clock = clock;
    }

    public async Task<ApplicationResult<HitDieDto>> ExecuteAsync(CreateHitDieCommand command, CancellationToken cancellationToken)
    {
        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);

        if (source.IsFailure) return ApplicationResult<HitDieDto>.Failure(source.Error);

        if (await hitDice.ExistsByDieAsync(command.Die, cancellationToken))
        {
            return ApplicationResult<HitDieDto>.Failure(FundamentalErrors.HitDieAlreadyExists(command.Die));
        }

        var hitDie = HitDie.Create(source.Value.RuleSourceId, source.Value.SourceVersionId, command.Die, clock.UtcNow);
        if (hitDie.IsFailure)
        {
            return ApplicationResult<HitDieDto>.Failure(FundamentalErrors.FromDomain(hitDie.Error));
        }

        await hitDice.AddAsync(hitDie.Value, cancellationToken);
        await hitDice.SaveChangesAsync(cancellationToken);
        return ApplicationResult<HitDieDto>.Success(hitDie.Value.ToDto());
    }
}

public sealed class ListHitDiceQuery
{
    private readonly IHitDieRepository repository;

    public ListHitDiceQuery(IHitDieRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<IReadOnlyCollection<HitDieDto>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var hitDice = await repository.ListAsync(cancellationToken);
        return ApplicationResult<IReadOnlyCollection<HitDieDto>>.Success(hitDice.Select(hitDie => hitDie.ToDto()).ToArray());
    }
}

internal static class ProficiencyTrainingMapping
{
    public static ProficiencyDto ToDto(this Proficiency proficiency) =>
        new(
            proficiency.Id.Value,
            proficiency.RuleSourceId.Value,
            proficiency.SourceVersionId.Value,
            proficiency.Code.Value,
            proficiency.Name.Value,
            proficiency.Type,
            proficiency.RelatedEntityId?.Value);

    public static ArmorTrainingCategoryDto ToDto(this ArmorTrainingCategory category) =>
        new(
            category.Id.Value,
            category.RuleSourceId.Value,
            category.SourceVersionId.Value,
            category.Code.Value,
            category.Name.Value,
            category.SortOrder);

    public static HitDieDto ToDto(this HitDie hitDie) =>
        new(hitDie.Id.Value, hitDie.RuleSourceId.Value, hitDie.SourceVersionId.Value, hitDie.Code.Value, hitDie.Name.Value, hitDie.Die);
}
