using Compendium.Application.Errors;
using Compendium.Application.Fundamentals;
using Compendium.Application.Sources;
using Compendium.Domain.Classes;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Classes;

public sealed class CreateClassUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IAbilityRepository abilities;
    private readonly IArmorTrainingCategoryRepository armorTrainingCategories;
    private readonly IHitDieRepository hitDice;
    private readonly IProficiencyRepository proficiencies;
    private readonly ICharacterClassRepository classes;
    private readonly IClock clock;

    public CreateClassUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IAbilityRepository abilities,
        IArmorTrainingCategoryRepository armorTrainingCategories,
        IHitDieRepository hitDice,
        IProficiencyRepository proficiencies,
        ICharacterClassRepository classes,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.abilities = abilities;
        this.armorTrainingCategories = armorTrainingCategories;
        this.hitDice = hitDice;
        this.proficiencies = proficiencies;
        this.classes = classes;
        this.clock = clock;
    }

    public async Task<ApplicationResult<ClassDetailsDto>> ExecuteAsync(CreateClassCommand command, CancellationToken cancellationToken)
    {
        var code = ClassCode.Create(command.Code);
        var name = ClassName.Create(command.Name);
        var description = ClassDescription.CreateOptional(command.Description);

        if (code.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.FromDomain(name.Error));
        if (description.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.FromDomain(description.Error));

        var source = await ClassReferenceValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        var coreTraits = await ClassReferenceValidation.ResolveCoreTraitsAsync(hitDice, armorTrainingCategories, command.CoreTraits, cancellationToken);
        var primaryAbilityIds = await ClassReferenceValidation.ResolveAbilityIdsAsync(abilities, command.PrimaryAbilityIds ?? [], cancellationToken);
        var levelInputs = await ClassReferenceValidation.ResolveLevelInputsAsync(proficiencies, command.Levels ?? [], cancellationToken);

        if (source.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(source.Error);
        if (coreTraits.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(coreTraits.Error);
        if (primaryAbilityIds.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(primaryAbilityIds.Error);
        if (levelInputs.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(levelInputs.Error);

        if (await classes.ExistsByCodeAsync(code.Value, cancellationToken))
        {
            return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.ClassCodeAlreadyExists(code.Value.Value));
        }

        var characterClass = CharacterClass.Create(
            source.Value.RuleSourceId,
            source.Value.SourceVersionId,
            code.Value,
            name.Value,
            description.Value,
            coreTraits.Value,
            primaryAbilityIds.Value,
            levelInputs.Value,
            clock.UtcNow);

        if (characterClass.IsFailure)
        {
            return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.FromDomain(characterClass.Error));
        }

        await classes.AddAsync(characterClass.Value, cancellationToken);
        await classes.SaveChangesAsync(cancellationToken);
        return ApplicationResult<ClassDetailsDto>.Success(characterClass.Value.ToDetailsDto());
    }
}

public sealed class UpdateClassUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IAbilityRepository abilities;
    private readonly IArmorTrainingCategoryRepository armorTrainingCategories;
    private readonly IHitDieRepository hitDice;
    private readonly ICharacterClassRepository classes;
    private readonly IClock clock;

    public UpdateClassUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IAbilityRepository abilities,
        IArmorTrainingCategoryRepository armorTrainingCategories,
        IHitDieRepository hitDice,
        ICharacterClassRepository classes,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.abilities = abilities;
        this.armorTrainingCategories = armorTrainingCategories;
        this.hitDice = hitDice;
        this.classes = classes;
        this.clock = clock;
    }

    public async Task<ApplicationResult<ClassDetailsDto>> ExecuteAsync(UpdateClassCommand command, CancellationToken cancellationToken)
    {
        var code = ClassCode.Create(command.Code);
        var name = ClassName.Create(command.Name);
        var description = ClassDescription.CreateOptional(command.Description);

        if (code.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.FromDomain(name.Error));
        if (description.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.FromDomain(description.Error));

        var source = await ClassReferenceValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        var coreTraits = await ClassReferenceValidation.ResolveCoreTraitsAsync(hitDice, armorTrainingCategories, command.CoreTraits, cancellationToken);
        var primaryAbilityIds = await ClassReferenceValidation.ResolveAbilityIdsAsync(abilities, command.PrimaryAbilityIds ?? [], cancellationToken);

        if (source.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(source.Error);
        if (coreTraits.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(coreTraits.Error);
        if (primaryAbilityIds.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(primaryAbilityIds.Error);

        var characterClass = await classes.GetByCodeAsync(code.Value, cancellationToken);
        if (characterClass is null)
        {
            return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.ClassNotFound(code.Value.Value));
        }

        var update = characterClass.Update(
            source.Value.SourceVersionId,
            name.Value,
            description.Value,
            coreTraits.Value,
            primaryAbilityIds.Value,
            clock.UtcNow);

        if (update.IsFailure)
        {
            return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.FromDomain(update.Error));
        }

        await classes.SaveChangesAsync(cancellationToken);
        return ApplicationResult<ClassDetailsDto>.Success(characterClass.ToDetailsDto());
    }
}

public sealed class ListClassesQuery
{
    private readonly ICharacterClassRepository repository;

    public ListClassesQuery(ICharacterClassRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<IReadOnlyCollection<ClassSummaryDto>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var classes = await repository.ListAsync(cancellationToken);
        return ApplicationResult<IReadOnlyCollection<ClassSummaryDto>>.Success(classes.Select(characterClass => characterClass.ToSummaryDto()).ToArray());
    }
}

public sealed class GetClassDetailsQuery
{
    private readonly ICharacterClassRepository repository;

    public GetClassDetailsQuery(ICharacterClassRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<ClassDetailsDto>> ExecuteAsync(string codeValue, CancellationToken cancellationToken)
    {
        var code = ClassCode.Create(codeValue);
        if (code.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.FromDomain(code.Error));

        var characterClass = await repository.GetByCodeAsync(code.Value, cancellationToken);
        return characterClass is null
            ? ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.ClassNotFound(code.Value.Value))
            : ApplicationResult<ClassDetailsDto>.Success(characterClass.ToDetailsDto());
    }
}

public sealed class ConfigureClassProgressionUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IAbilityRepository abilities;
    private readonly IProficiencyRepository proficiencies;
    private readonly ICharacterClassRepository classes;
    private readonly IClock clock;

    public ConfigureClassProgressionUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IAbilityRepository abilities,
        IProficiencyRepository proficiencies,
        ICharacterClassRepository classes,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.abilities = abilities;
        this.proficiencies = proficiencies;
        this.classes = classes;
        this.clock = clock;
    }

    public async Task<ApplicationResult<ClassDetailsDto>> ExecuteAsync(ConfigureClassProgressionCommand command, CancellationToken cancellationToken)
    {
        var code = ClassCode.Create(command.Code);

        if (code.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.FromDomain(code.Error));

        var source = await ClassReferenceValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        var levelInputs = await ClassReferenceValidation.ResolveLevelInputsAsync(proficiencies, command.Levels ?? [], cancellationToken);
        var spellcasting = await ClassReferenceValidation.ResolveSpellcastingProgressionAsync(abilities, command.SpellcastingProgression, cancellationToken);

        if (source.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(source.Error);
        if (levelInputs.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(levelInputs.Error);
        if (spellcasting.IsFailure) return ApplicationResult<ClassDetailsDto>.Failure(spellcasting.Error);

        var characterClass = await classes.GetByCodeAsync(code.Value, cancellationToken);
        if (characterClass is null)
        {
            return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.ClassNotFound(code.Value.Value));
        }

        var configure = characterClass.ConfigureProgression(levelInputs.Value, spellcasting.Value, source.Value.SourceVersionId, clock.UtcNow);
        if (configure.IsFailure)
        {
            return ApplicationResult<ClassDetailsDto>.Failure(ClassErrors.FromDomain(configure.Error));
        }

        await classes.SaveChangesAsync(cancellationToken);
        return ApplicationResult<ClassDetailsDto>.Success(characterClass.ToDetailsDto());
    }
}

public sealed class GetClassProgressionQuery
{
    private readonly ICharacterClassRepository repository;

    public GetClassProgressionQuery(ICharacterClassRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<ClassProgressionDto>> ExecuteAsync(string codeValue, CancellationToken cancellationToken)
    {
        var code = ClassCode.Create(codeValue);
        if (code.IsFailure) return ApplicationResult<ClassProgressionDto>.Failure(ClassErrors.FromDomain(code.Error));

        var characterClass = await repository.GetByCodeAsync(code.Value, cancellationToken);
        return characterClass is null
            ? ApplicationResult<ClassProgressionDto>.Failure(ClassErrors.ClassNotFound(code.Value.Value))
            : ApplicationResult<ClassProgressionDto>.Success(characterClass.ToProgressionDto());
    }
}

public sealed class CreateSubclassUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly ICharacterClassRepository classes;
    private readonly ICharacterSubclassRepository subclasses;
    private readonly IClock clock;

    public CreateSubclassUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        ICharacterClassRepository classes,
        ICharacterSubclassRepository subclasses,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.classes = classes;
        this.subclasses = subclasses;
        this.clock = clock;
    }

    public async Task<ApplicationResult<SubclassDetailsDto>> ExecuteAsync(CreateSubclassCommand command, CancellationToken cancellationToken)
    {
        var classCode = ClassCode.Create(command.ClassCode);
        var subclassCode = ClassCode.Create(command.Code);
        var name = ClassName.Create(command.Name);
        var description = ClassDescription.CreateOptional(command.Description);

        if (classCode.IsFailure) return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.FromDomain(classCode.Error));
        if (subclassCode.IsFailure) return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.FromDomain(subclassCode.Error));
        if (name.IsFailure) return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.FromDomain(name.Error));
        if (description.IsFailure) return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.FromDomain(description.Error));

        var source = await ClassReferenceValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<SubclassDetailsDto>.Failure(source.Error);

        var characterClass = await classes.GetByCodeAsync(classCode.Value, cancellationToken);
        if (characterClass is null)
        {
            return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.ClassNotFound(classCode.Value.Value));
        }

        if (await subclasses.ExistsByClassAndCodeAsync(characterClass.Id, subclassCode.Value, cancellationToken))
        {
            return ApplicationResult<SubclassDetailsDto>.Failure(
                ClassErrors.SubclassCodeAlreadyExists(classCode.Value.Value, subclassCode.Value.Value));
        }

        var subclass = CharacterSubclass.Create(
            characterClass.Id,
            source.Value.RuleSourceId,
            source.Value.SourceVersionId,
            subclassCode.Value,
            name.Value,
            description.Value,
            clock.UtcNow);

        await subclasses.AddAsync(subclass.Value, cancellationToken);
        await subclasses.SaveChangesAsync(cancellationToken);
        return ApplicationResult<SubclassDetailsDto>.Success(subclass.Value.ToDetailsDto());
    }
}

public sealed class LinkSubclassFeatureUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly ICharacterClassRepository classes;
    private readonly ICharacterSubclassRepository subclasses;
    private readonly IClock clock;

    public LinkSubclassFeatureUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        ICharacterClassRepository classes,
        ICharacterSubclassRepository subclasses,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.classes = classes;
        this.subclasses = subclasses;
        this.clock = clock;
    }

    public async Task<ApplicationResult<SubclassDetailsDto>> ExecuteAsync(LinkSubclassFeatureCommand command, CancellationToken cancellationToken)
    {
        var classCode = ClassCode.Create(command.ClassCode);
        var subclassCode = ClassCode.Create(command.SubclassCode);
        var featureId = CompendiumEntityId.Create(command.FeatureId);

        if (classCode.IsFailure) return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.FromDomain(classCode.Error));
        if (subclassCode.IsFailure) return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.FromDomain(subclassCode.Error));
        if (featureId.IsFailure) return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.FromDomain(featureId.Error));

        var source = await ClassReferenceValidation.ValidateSourceAsync(sources, versions, command.RuleSourceId, command.SourceVersionId, cancellationToken);
        if (source.IsFailure) return ApplicationResult<SubclassDetailsDto>.Failure(source.Error);

        var characterClass = await classes.GetByCodeAsync(classCode.Value, cancellationToken);
        if (characterClass is null)
        {
            return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.ClassNotFound(classCode.Value.Value));
        }

        var subclass = await subclasses.GetByClassAndCodeAsync(characterClass.Id, subclassCode.Value, cancellationToken);
        if (subclass is null)
        {
            return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.SubclassNotFound(classCode.Value.Value, subclassCode.Value.Value));
        }

        var link = subclass.LinkFeature(featureId.Value, source.Value.SourceVersionId, command.Level, clock.UtcNow);
        if (link.IsFailure)
        {
            return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.FromDomain(link.Error));
        }

        await subclasses.SaveChangesAsync(cancellationToken);
        return ApplicationResult<SubclassDetailsDto>.Success(subclass.ToDetailsDto());
    }
}

public sealed class ListSubclassesByClassQuery
{
    private readonly ICharacterClassRepository classes;
    private readonly ICharacterSubclassRepository subclasses;

    public ListSubclassesByClassQuery(ICharacterClassRepository classes, ICharacterSubclassRepository subclasses)
    {
        this.classes = classes;
        this.subclasses = subclasses;
    }

    public async Task<ApplicationResult<IReadOnlyCollection<SubclassSummaryDto>>> ExecuteAsync(string classCodeValue, CancellationToken cancellationToken)
    {
        var classCode = ClassCode.Create(classCodeValue);
        if (classCode.IsFailure) return ApplicationResult<IReadOnlyCollection<SubclassSummaryDto>>.Failure(ClassErrors.FromDomain(classCode.Error));

        var characterClass = await classes.GetByCodeAsync(classCode.Value, cancellationToken);
        if (characterClass is null)
        {
            return ApplicationResult<IReadOnlyCollection<SubclassSummaryDto>>.Failure(ClassErrors.ClassNotFound(classCode.Value.Value));
        }

        var result = await subclasses.ListByClassAsync(characterClass.Id, cancellationToken);
        return ApplicationResult<IReadOnlyCollection<SubclassSummaryDto>>.Success(result.Select(subclass => subclass.ToSummaryDto()).ToArray());
    }
}

public sealed class GetSubclassDetailsQuery
{
    private readonly ICharacterClassRepository classes;
    private readonly ICharacterSubclassRepository subclasses;

    public GetSubclassDetailsQuery(ICharacterClassRepository classes, ICharacterSubclassRepository subclasses)
    {
        this.classes = classes;
        this.subclasses = subclasses;
    }

    public async Task<ApplicationResult<SubclassDetailsDto>> ExecuteAsync(
        string classCodeValue,
        string subclassCodeValue,
        CancellationToken cancellationToken)
    {
        var classCode = ClassCode.Create(classCodeValue);
        var subclassCode = ClassCode.Create(subclassCodeValue);

        if (classCode.IsFailure) return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.FromDomain(classCode.Error));
        if (subclassCode.IsFailure) return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.FromDomain(subclassCode.Error));

        var characterClass = await classes.GetByCodeAsync(classCode.Value, cancellationToken);
        if (characterClass is null)
        {
            return ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.ClassNotFound(classCode.Value.Value));
        }

        var subclass = await subclasses.GetByClassAndCodeAsync(characterClass.Id, subclassCode.Value, cancellationToken);
        return subclass is null
            ? ApplicationResult<SubclassDetailsDto>.Failure(ClassErrors.SubclassNotFound(classCode.Value.Value, subclassCode.Value.Value))
            : ApplicationResult<SubclassDetailsDto>.Success(subclass.ToDetailsDto());
    }
}

internal static class ClassReferenceValidation
{
    public static Task<ApplicationResult<SourceReference>> ValidateSourceAsync(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        Guid ruleSourceId,
        Guid sourceVersionId,
        CancellationToken cancellationToken) =>
        FundamentalSourceReference.ValidateAsync(sources, versions, ruleSourceId, sourceVersionId, cancellationToken);

    public static async Task<ApplicationResult<ClassCoreTraitsInput>> ResolveCoreTraitsAsync(
        IHitDieRepository hitDice,
        IArmorTrainingCategoryRepository armorTrainingCategories,
        ClassCoreTraitsCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
        {
            return ApplicationResult<ClassCoreTraitsInput>.Failure(ClassErrors.FromDomain(ClassDomainErrors.CoreTraitsRequired()));
        }

        var hitDieId = CompendiumEntityId.Create(command.HitDieId);
        if (hitDieId.IsFailure)
        {
            return ApplicationResult<ClassCoreTraitsInput>.Failure(ClassErrors.FromDomain(hitDieId.Error));
        }

        if (await hitDice.GetByIdAsync(hitDieId.Value, cancellationToken) is null)
        {
            return ApplicationResult<ClassCoreTraitsInput>.Failure(ClassErrors.HitDieNotFound(hitDieId.Value.ToString()));
        }

        CompendiumEntityId? armorTrainingCategoryId = null;
        if (command.ArmorTrainingCategoryId.HasValue)
        {
            var parsed = CompendiumEntityId.Create(command.ArmorTrainingCategoryId.Value);
            if (parsed.IsFailure)
            {
                return ApplicationResult<ClassCoreTraitsInput>.Failure(ClassErrors.FromDomain(parsed.Error));
            }

            if (await armorTrainingCategories.GetByIdAsync(parsed.Value, cancellationToken) is null)
            {
                return ApplicationResult<ClassCoreTraitsInput>.Failure(ClassErrors.ArmorTrainingCategoryNotFound(parsed.Value.ToString()));
            }

            armorTrainingCategoryId = parsed.Value;
        }

        return ApplicationResult<ClassCoreTraitsInput>.Success(new ClassCoreTraitsInput(
            hitDieId.Value,
            armorTrainingCategoryId,
            command.SkillChoiceCount));
    }

    public static async Task<ApplicationResult<IReadOnlyCollection<CompendiumEntityId>>> ResolveAbilityIdsAsync(
        IAbilityRepository abilities,
        IReadOnlyCollection<Guid> abilityIdValues,
        CancellationToken cancellationToken)
    {
        var abilityIds = new List<CompendiumEntityId>();
        foreach (var abilityIdValue in abilityIdValues)
        {
            var abilityId = CompendiumEntityId.Create(abilityIdValue);
            if (abilityId.IsFailure)
            {
                return ApplicationResult<IReadOnlyCollection<CompendiumEntityId>>.Failure(ClassErrors.FromDomain(abilityId.Error));
            }

            if (await abilities.GetByIdAsync(abilityId.Value, cancellationToken) is null)
            {
                return ApplicationResult<IReadOnlyCollection<CompendiumEntityId>>.Failure(ClassErrors.AbilityNotFound(abilityId.Value.ToString()));
            }

            abilityIds.Add(abilityId.Value);
        }

        return ApplicationResult<IReadOnlyCollection<CompendiumEntityId>>.Success(abilityIds);
    }

    public static async Task<ApplicationResult<IReadOnlyCollection<ClassLevelInput>>> ResolveLevelInputsAsync(
        IProficiencyRepository proficiencies,
        IReadOnlyCollection<ClassLevelCommand> commands,
        CancellationToken cancellationToken)
    {
        var levelInputs = new List<ClassLevelInput>();
        foreach (var command in commands)
        {
            var proficiencyIds = new List<CompendiumEntityId>();
            foreach (var proficiencyIdValue in command.ProficiencyGrantIds ?? [])
            {
                var proficiencyId = CompendiumEntityId.Create(proficiencyIdValue);
                if (proficiencyId.IsFailure)
                {
                    return ApplicationResult<IReadOnlyCollection<ClassLevelInput>>.Failure(ClassErrors.FromDomain(proficiencyId.Error));
                }

                if (!await proficiencies.ExistsByIdAsync(proficiencyId.Value, cancellationToken))
                {
                    return ApplicationResult<IReadOnlyCollection<ClassLevelInput>>.Failure(ClassErrors.ProficiencyNotFound(proficiencyId.Value.ToString()));
                }

                proficiencyIds.Add(proficiencyId.Value);
            }

            levelInputs.Add(new ClassLevelInput(
                command.Level,
                command.ProficiencyBonus,
                (command.SpellSlots ?? [])
                    .Select(slot => new ClassLevelSpellSlotInput(slot.SpellLevel, slot.Slots))
                    .ToArray(),
                proficiencyIds,
                command.WeaponMasteryCount));
        }

        return ApplicationResult<IReadOnlyCollection<ClassLevelInput>>.Success(levelInputs);
    }

    public static async Task<ApplicationResult<ClassSpellcastingProgressionInput?>> ResolveSpellcastingProgressionAsync(
        IAbilityRepository abilities,
        ClassSpellcastingProgressionCommand? command,
        CancellationToken cancellationToken)
    {
        if (command is null)
        {
            return ApplicationResult<ClassSpellcastingProgressionInput?>.Success(null);
        }

        CompendiumEntityId? spellcastingAbilityId = null;
        if (command.SpellcastingAbilityId.HasValue)
        {
            var abilityId = CompendiumEntityId.Create(command.SpellcastingAbilityId.Value);
            if (abilityId.IsFailure)
            {
                return ApplicationResult<ClassSpellcastingProgressionInput?>.Failure(ClassErrors.FromDomain(abilityId.Error));
            }

            if (await abilities.GetByIdAsync(abilityId.Value, cancellationToken) is null)
            {
                return ApplicationResult<ClassSpellcastingProgressionInput?>.Failure(ClassErrors.AbilityNotFound(abilityId.Value.ToString()));
            }

            spellcastingAbilityId = abilityId.Value;
        }

        return ApplicationResult<ClassSpellcastingProgressionInput?>.Success(new ClassSpellcastingProgressionInput(
            command.Type,
            spellcastingAbilityId,
            (command.LevelRules ?? [])
                .Select(rule => new ClassSpellcastingLevelRuleInput(rule.ClassLevel, rule.CasterLevel))
                .ToArray()));
    }
}

internal static class ClassMapping
{
    public static ClassSummaryDto ToSummaryDto(this CharacterClass characterClass) =>
        new(
            characterClass.Id.Value,
            characterClass.RuleSourceId.Value,
            characterClass.SourceVersionId.Value,
            characterClass.Code.Value,
            characterClass.Name.Value,
            characterClass.Description?.Value);

    public static ClassDetailsDto ToDetailsDto(this CharacterClass characterClass) =>
        new(
            characterClass.Id.Value,
            characterClass.RuleSourceId.Value,
            characterClass.SourceVersionId.Value,
            characterClass.Code.Value,
            characterClass.Name.Value,
            characterClass.Description?.Value,
            new ClassCoreTraitsDto(
                characterClass.CoreTraits!.Id.Value,
                characterClass.CoreTraits.HitDieId.Value,
                characterClass.CoreTraits.ArmorTrainingCategoryId?.Value,
                characterClass.CoreTraits.SkillChoiceCount),
            characterClass.PrimaryAbilities
                .OrderBy(ability => ability.SortOrder)
                .Select(ability => new ClassPrimaryAbilityDto(ability.Id.Value, ability.AbilityId.Value, ability.SortOrder))
                .ToArray(),
            characterClass.Levels.ToLevelDtos(),
            characterClass.SpellcastingProgression?.ToDto());

    public static IReadOnlyCollection<ClassLevelDto> ToLevelDtos(this IEnumerable<ClassLevel> levels) =>
        levels
            .OrderBy(level => level.Level)
            .Select(level => new ClassLevelDto(
                level.Id.Value,
                level.Level,
                level.ProficiencyBonus,
                level.SpellSlots
                    .OrderBy(slot => slot.SpellLevel)
                    .Select(slot => new ClassLevelSpellSlotDto(slot.Id.Value, slot.SpellLevel, slot.Slots))
                    .ToArray(),
                level.ProficiencyGrants
                    .Select(grant => new ClassProficiencyGrantDto(grant.Id.Value, grant.ProficiencyId.Value))
                    .ToArray(),
                level.WeaponMasteryCounts.SingleOrDefault()?.Count))
            .ToArray();

    public static ClassProgressionDto ToProgressionDto(this CharacterClass characterClass) =>
        new(characterClass.Levels.ToLevelDtos(), characterClass.SpellcastingProgression?.ToDto());

    public static ClassSpellcastingProgressionDto ToDto(this ClassSpellcastingProgression progression) =>
        new(
            progression.Id.Value,
            progression.Type,
            progression.SpellcastingAbilityId?.Value,
            progression.LevelRules
                .OrderBy(rule => rule.ClassLevel)
                .Select(rule => new ClassSpellcastingLevelRuleDto(rule.Id.Value, rule.ClassLevel, rule.CasterLevel))
                .ToArray());

    public static SubclassSummaryDto ToSummaryDto(this CharacterSubclass subclass) =>
        new(
            subclass.Id.Value,
            subclass.CharacterClassId.Value,
            subclass.RuleSourceId.Value,
            subclass.SourceVersionId.Value,
            subclass.Code.Value,
            subclass.Name.Value,
            subclass.Description?.Value);

    public static SubclassDetailsDto ToDetailsDto(this CharacterSubclass subclass) =>
        new(
            subclass.Id.Value,
            subclass.CharacterClassId.Value,
            subclass.RuleSourceId.Value,
            subclass.SourceVersionId.Value,
            subclass.Code.Value,
            subclass.Name.Value,
            subclass.Description?.Value,
            subclass.Features
                .OrderBy(feature => feature.Level)
                .Select(feature => new SubclassFeatureDto(feature.Id.Value, feature.SourceVersionId.Value, feature.FeatureId.Value, feature.Level))
                .ToArray());
}
