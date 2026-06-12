using Compendium.Application.Errors;
using Compendium.Application.Sources;
using Compendium.Domain.Fundamentals;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Fundamentals;

public sealed class CreateAbilityUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IAbilityRepository abilities;
    private readonly IClock clock;

    public CreateAbilityUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IAbilityRepository abilities,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.abilities = abilities;
        this.clock = clock;
    }

    public async Task<ApplicationResult<AbilityDto>> ExecuteAsync(CreateAbilityCommand command, CancellationToken cancellationToken)
    {
        var code = AbilityCode.Create(command.Code);
        var name = DisplayName.Create(command.Name);

        if (code.IsFailure) return ApplicationResult<AbilityDto>.Failure(FundamentalErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<AbilityDto>.Failure(FundamentalErrors.FromDomain(name.Error));

        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<AbilityDto>.Failure(source.Error);

        if (await abilities.ExistsByCodeAsync(code.Value, cancellationToken))
        {
            return ApplicationResult<AbilityDto>.Failure(FundamentalErrors.AbilityCodeAlreadyExists(code.Value.Value));
        }

        var ability = Ability.Create(source.Value.RuleSourceId, source.Value.SourceVersionId, code.Value, name.Value, clock.UtcNow);
        await abilities.AddAsync(ability.Value, cancellationToken);
        await abilities.SaveChangesAsync(cancellationToken);
        return ApplicationResult<AbilityDto>.Success(ability.Value.ToDto());
    }
}

public sealed class UpdateAbilityUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IAbilityRepository abilities;
    private readonly IClock clock;

    public UpdateAbilityUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IAbilityRepository abilities,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.abilities = abilities;
        this.clock = clock;
    }

    public async Task<ApplicationResult<AbilityDto>> ExecuteAsync(UpdateAbilityCommand command, CancellationToken cancellationToken)
    {
        var code = AbilityCode.Create(command.Code);
        var name = DisplayName.Create(command.Name);

        if (code.IsFailure) return ApplicationResult<AbilityDto>.Failure(FundamentalErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<AbilityDto>.Failure(FundamentalErrors.FromDomain(name.Error));

        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<AbilityDto>.Failure(source.Error);

        var ability = await abilities.GetByCodeAsync(code.Value, cancellationToken);
        if (ability is null)
        {
            return ApplicationResult<AbilityDto>.Failure(FundamentalErrors.AbilityNotFound(code.Value.Value));
        }

        ability.Update(name.Value, source.Value.SourceVersionId, clock.UtcNow);
        await abilities.SaveChangesAsync(cancellationToken);
        return ApplicationResult<AbilityDto>.Success(ability.ToDto());
    }
}

public sealed class ListAbilitiesQuery
{
    private readonly IAbilityRepository repository;

    public ListAbilitiesQuery(IAbilityRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<IReadOnlyCollection<AbilityDto>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var abilities = await repository.ListAsync(cancellationToken);
        return ApplicationResult<IReadOnlyCollection<AbilityDto>>.Success(abilities.Select(ability => ability.ToDto()).ToArray());
    }
}

public sealed class CreateSkillUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IAbilityRepository abilities;
    private readonly ISkillRepository skills;
    private readonly IClock clock;

    public CreateSkillUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IAbilityRepository abilities,
        ISkillRepository skills,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.abilities = abilities;
        this.skills = skills;
        this.clock = clock;
    }

    public async Task<ApplicationResult<SkillDto>> ExecuteAsync(CreateSkillCommand command, CancellationToken cancellationToken)
    {
        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        var code = SkillCode.Create(command.Code);
        var name = DisplayName.Create(command.Name);
        var defaultAbilityId = await ResolveAbilityIdAsync(command.DefaultAbilityId, cancellationToken);

        if (source.IsFailure) return ApplicationResult<SkillDto>.Failure(source.Error);
        if (code.IsFailure) return ApplicationResult<SkillDto>.Failure(FundamentalErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<SkillDto>.Failure(FundamentalErrors.FromDomain(name.Error));
        if (defaultAbilityId.IsFailure) return ApplicationResult<SkillDto>.Failure(defaultAbilityId.Error);

        if (await skills.ExistsByCodeAsync(code.Value, cancellationToken))
        {
            return ApplicationResult<SkillDto>.Failure(FundamentalErrors.SkillCodeAlreadyExists(code.Value.Value));
        }

        var skill = Skill.Create(
            source.Value.RuleSourceId,
            source.Value.SourceVersionId,
            code.Value,
            name.Value,
            defaultAbilityId.Value,
            clock.UtcNow);

        await skills.AddAsync(skill.Value, cancellationToken);
        await skills.SaveChangesAsync(cancellationToken);
        return ApplicationResult<SkillDto>.Success(skill.Value.ToDto());
    }

    private async Task<ApplicationResult<CompendiumEntityId?>> ResolveAbilityIdAsync(Guid? abilityIdValue, CancellationToken cancellationToken)
    {
        if (!abilityIdValue.HasValue)
        {
            return ApplicationResult<CompendiumEntityId?>.Success(null);
        }

        var abilityId = CompendiumEntityId.Create(abilityIdValue.Value);
        if (abilityId.IsFailure)
        {
            return ApplicationResult<CompendiumEntityId?>.Failure(FundamentalErrors.FromDomain(abilityId.Error));
        }

        return await abilities.GetByIdAsync(abilityId.Value, cancellationToken) is null
            ? ApplicationResult<CompendiumEntityId?>.Failure(FundamentalErrors.AbilityNotFound(abilityId.Value.ToString()))
            : ApplicationResult<CompendiumEntityId?>.Success(abilityId.Value);
    }
}

public sealed class UpdateSkillUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IAbilityRepository abilities;
    private readonly ISkillRepository skills;
    private readonly IClock clock;

    public UpdateSkillUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IAbilityRepository abilities,
        ISkillRepository skills,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.abilities = abilities;
        this.skills = skills;
        this.clock = clock;
    }

    public async Task<ApplicationResult<SkillDto>> ExecuteAsync(UpdateSkillCommand command, CancellationToken cancellationToken)
    {
        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        var code = SkillCode.Create(command.Code);
        var name = DisplayName.Create(command.Name);
        var defaultAbilityId = await ResolveAbilityIdAsync(command.DefaultAbilityId, cancellationToken);

        if (source.IsFailure) return ApplicationResult<SkillDto>.Failure(source.Error);
        if (code.IsFailure) return ApplicationResult<SkillDto>.Failure(FundamentalErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<SkillDto>.Failure(FundamentalErrors.FromDomain(name.Error));
        if (defaultAbilityId.IsFailure) return ApplicationResult<SkillDto>.Failure(defaultAbilityId.Error);

        var skill = await skills.GetByCodeAsync(code.Value, cancellationToken);
        if (skill is null)
        {
            return ApplicationResult<SkillDto>.Failure(FundamentalErrors.SkillNotFound(code.Value.Value));
        }

        skill.Update(name.Value, source.Value.SourceVersionId, defaultAbilityId.Value, clock.UtcNow);
        await skills.SaveChangesAsync(cancellationToken);
        return ApplicationResult<SkillDto>.Success(skill.ToDto());
    }

    private async Task<ApplicationResult<CompendiumEntityId?>> ResolveAbilityIdAsync(Guid? abilityIdValue, CancellationToken cancellationToken)
    {
        if (!abilityIdValue.HasValue)
        {
            return ApplicationResult<CompendiumEntityId?>.Success(null);
        }

        var abilityId = CompendiumEntityId.Create(abilityIdValue.Value);
        if (abilityId.IsFailure)
        {
            return ApplicationResult<CompendiumEntityId?>.Failure(FundamentalErrors.FromDomain(abilityId.Error));
        }

        return await abilities.GetByIdAsync(abilityId.Value, cancellationToken) is null
            ? ApplicationResult<CompendiumEntityId?>.Failure(FundamentalErrors.AbilityNotFound(abilityId.Value.ToString()))
            : ApplicationResult<CompendiumEntityId?>.Success(abilityId.Value);
    }
}

public sealed class ListSkillsQuery
{
    private readonly ISkillRepository repository;

    public ListSkillsQuery(ISkillRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<IReadOnlyCollection<SkillDto>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var skills = await repository.ListAsync(cancellationToken);
        return ApplicationResult<IReadOnlyCollection<SkillDto>>.Success(skills.Select(skill => skill.ToDto()).ToArray());
    }
}

public sealed class CreateLanguageUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly ILanguageRepository languages;
    private readonly IClock clock;

    public CreateLanguageUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        ILanguageRepository languages,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.languages = languages;
        this.clock = clock;
    }

    public async Task<ApplicationResult<LanguageDto>> ExecuteAsync(CreateLanguageCommand command, CancellationToken cancellationToken)
    {
        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        var code = LanguageCode.Create(command.Code);
        var name = DisplayName.Create(command.Name);

        if (source.IsFailure) return ApplicationResult<LanguageDto>.Failure(source.Error);
        if (code.IsFailure) return ApplicationResult<LanguageDto>.Failure(FundamentalErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<LanguageDto>.Failure(FundamentalErrors.FromDomain(name.Error));

        if (await languages.ExistsByCodeAsync(code.Value, cancellationToken))
        {
            return ApplicationResult<LanguageDto>.Failure(FundamentalErrors.LanguageCodeAlreadyExists(code.Value.Value));
        }

        var language = Language.Create(source.Value.RuleSourceId, source.Value.SourceVersionId, code.Value, name.Value, clock.UtcNow);
        await languages.AddAsync(language.Value, cancellationToken);
        await languages.SaveChangesAsync(cancellationToken);
        return ApplicationResult<LanguageDto>.Success(language.Value.ToDto());
    }
}

public sealed class UpdateLanguageUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly ILanguageRepository languages;
    private readonly IClock clock;

    public UpdateLanguageUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        ILanguageRepository languages,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.languages = languages;
        this.clock = clock;
    }

    public async Task<ApplicationResult<LanguageDto>> ExecuteAsync(UpdateLanguageCommand command, CancellationToken cancellationToken)
    {
        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        var code = LanguageCode.Create(command.Code);
        var name = DisplayName.Create(command.Name);

        if (source.IsFailure) return ApplicationResult<LanguageDto>.Failure(source.Error);
        if (code.IsFailure) return ApplicationResult<LanguageDto>.Failure(FundamentalErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<LanguageDto>.Failure(FundamentalErrors.FromDomain(name.Error));

        var language = await languages.GetByCodeAsync(code.Value, cancellationToken);
        if (language is null)
        {
            return ApplicationResult<LanguageDto>.Failure(FundamentalErrors.LanguageNotFound(code.Value.Value));
        }

        language.Update(name.Value, source.Value.SourceVersionId, clock.UtcNow);
        await languages.SaveChangesAsync(cancellationToken);
        return ApplicationResult<LanguageDto>.Success(language.ToDto());
    }
}

public sealed class ListLanguagesQuery
{
    private readonly ILanguageRepository repository;

    public ListLanguagesQuery(ILanguageRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<IReadOnlyCollection<LanguageDto>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var languages = await repository.ListAsync(cancellationToken);
        return ApplicationResult<IReadOnlyCollection<LanguageDto>>.Success(languages.Select(language => language.ToDto()).ToArray());
    }
}

internal static class AbilitySkillLanguageMapping
{
    public static AbilityDto ToDto(this Ability ability) =>
        new(ability.Id.Value, ability.RuleSourceId.Value, ability.SourceVersionId.Value, ability.Code.Value, ability.Name.Value);

    public static SkillDto ToDto(this Skill skill) =>
        new(
            skill.Id.Value,
            skill.RuleSourceId.Value,
            skill.SourceVersionId.Value,
            skill.Code.Value,
            skill.Name.Value,
            skill.DefaultAbilityId?.Value);

    public static LanguageDto ToDto(this Language language) =>
        new(language.Id.Value, language.RuleSourceId.Value, language.SourceVersionId.Value, language.Code.Value, language.Name.Value);
}
