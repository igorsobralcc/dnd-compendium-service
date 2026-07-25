using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Classes;

public sealed class CharacterClass
{
    public const int MinSupportedLevel = 1;
    public const int MaxSupportedLevel = 20;

    private readonly List<ClassPrimaryAbility> primaryAbilities = [];
    private readonly List<ClassLevel> levels = [];

    private CharacterClass()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private CharacterClass(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        ClassCode code,
        ClassName name,
        ClassDescription? description)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
        Description = description;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId RuleSourceId { get; private set; }

    public CompendiumEntityId SourceVersionId { get; private set; }

    public ClassCode Code { get; private set; }

    public ClassName Name { get; private set; }

    public ClassDescription? Description { get; private set; }

    public ClassCoreTraits? CoreTraits { get; private set; }

    public IReadOnlyCollection<ClassPrimaryAbility> PrimaryAbilities => primaryAbilities;

    public IReadOnlyCollection<ClassLevel> Levels => levels;

    public ClassSpellcastingProgression? SpellcastingProgression { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<CharacterClass> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        ClassCode code,
        ClassName name,
        ClassDescription? description,
        ClassCoreTraitsInput coreTraitsInput,
        IReadOnlyCollection<CompendiumEntityId> primaryAbilityIds,
        IReadOnlyCollection<ClassLevelInput> levelInputs,
        DateTimeOffset now)
    {
        var characterClass = new CharacterClass(CompendiumEntityId.New(), ruleSourceId, sourceVersionId, code, name, description)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        var coreTraitsResult = characterClass.SetCoreTraits(coreTraitsInput);
        if (coreTraitsResult.IsFailure) return Result<CharacterClass>.Failure(coreTraitsResult.Error);

        var abilitiesResult = characterClass.ReplacePrimaryAbilities(primaryAbilityIds);
        if (abilitiesResult.IsFailure) return Result<CharacterClass>.Failure(abilitiesResult.Error);

        var levelsResult = characterClass.ReplaceProgression(levelInputs, null);
        return levelsResult.IsFailure
            ? Result<CharacterClass>.Failure(levelsResult.Error)
            : Result<CharacterClass>.Success(characterClass);
    }

    public Result Update(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        ClassName name,
        ClassDescription? description,
        ClassCoreTraitsInput coreTraitsInput,
        IReadOnlyCollection<CompendiumEntityId> primaryAbilityIds,
        DateTimeOffset now)
    {
        var coreTraitsResult = SetCoreTraits(coreTraitsInput);
        if (coreTraitsResult.IsFailure) return coreTraitsResult;

        var abilitiesResult = ReplacePrimaryAbilities(primaryAbilityIds);
        if (abilitiesResult.IsFailure) return abilitiesResult;

        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Name = name;
        Description = description;
        UpdatedAtUtc = now;
        return Result.Success();
    }

    public Result ConfigureProgression(
        IReadOnlyCollection<ClassLevelInput> levelInputs,
        ClassSpellcastingProgressionInput? spellcastingProgressionInput,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        DateTimeOffset now)
    {
        var result = ReplaceProgression(levelInputs, spellcastingProgressionInput);
        if (result.IsFailure) return result;

        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        UpdatedAtUtc = now;
        return Result.Success();
    }

    public static bool IsSupportedLevel(int level) =>
        level is >= MinSupportedLevel and <= MaxSupportedLevel;

    private Result SetCoreTraits(ClassCoreTraitsInput coreTraitsInput)
    {
        if (coreTraitsInput is null)
        {
            return Result.Failure(ClassDomainErrors.CoreTraitsRequired());
        }

        if (coreTraitsInput.SkillChoiceCount < 0)
        {
            return Result.Failure(ClassDomainErrors.InvalidSkillChoiceCount());
        }

        if (CoreTraits is null)
        {
            CoreTraits = ClassCoreTraits.Create(
                CompendiumEntityId.New(),
                Id,
                coreTraitsInput.HitDieId,
                coreTraitsInput.ArmorTrainingCategoryId,
                coreTraitsInput.SkillChoiceCount);
        }
        else
        {
            CoreTraits.Update(
                coreTraitsInput.HitDieId,
                coreTraitsInput.ArmorTrainingCategoryId,
                coreTraitsInput.SkillChoiceCount);
        }

        return Result.Success();
    }

    private Result ReplacePrimaryAbilities(IReadOnlyCollection<CompendiumEntityId> abilityIds)
    {
        if (abilityIds.Count == 0)
        {
            return Result.Failure(ClassDomainErrors.PrimaryAbilitiesRequired());
        }

        var seenAbilityIds = new HashSet<CompendiumEntityId>();
        var replacement = new List<ClassPrimaryAbility>();
        var sortOrder = 1;
        foreach (var abilityId in abilityIds)
        {
            if (!seenAbilityIds.Add(abilityId))
            {
                return Result.Failure(ClassDomainErrors.DuplicatePrimaryAbility(abilityId.ToString()));
            }

            replacement.Add(ClassPrimaryAbility.Create(CompendiumEntityId.New(), Id, abilityId, sortOrder++));
        }

        primaryAbilities.Clear();
        primaryAbilities.AddRange(replacement);
        return Result.Success();
    }

    private Result ReplaceProgression(
        IReadOnlyCollection<ClassLevelInput> levelInputs,
        ClassSpellcastingProgressionInput? spellcastingProgressionInput)
    {
        if (levelInputs.Count == 0)
        {
            return Result.Failure(ClassDomainErrors.LevelsRequired());
        }

        var seenLevels = new HashSet<int>();
        var replacementLevels = new List<ClassLevel>();
        foreach (var levelInput in levelInputs.OrderBy(input => input.Level))
        {
            if (!IsSupportedLevel(levelInput.Level))
            {
                return Result.Failure(ClassDomainErrors.InvalidLevel(levelInput.Level));
            }

            if (!seenLevels.Add(levelInput.Level))
            {
                return Result.Failure(ClassDomainErrors.DuplicateLevel(levelInput.Level));
            }

            var level = ClassLevel.Create(
                CompendiumEntityId.New(),
                Id,
                levelInput.Level,
                levelInput.ProficiencyBonus,
                levelInput.SpellSlots,
                levelInput.ProficiencyGrantIds,
                levelInput.WeaponMasteryCount);

            if (level.IsFailure)
            {
                return Result.Failure(level.Error);
            }

            replacementLevels.Add(level.Value);
        }

        var spellcasting = spellcastingProgressionInput is null
            ? Result<ClassSpellcastingProgression?>.Success(null)
            : ClassSpellcastingProgression.Create(CompendiumEntityId.New(), Id, spellcastingProgressionInput);
        if (spellcasting.IsFailure)
        {
            return Result.Failure(spellcasting.Error);
        }

        levels.Clear();
        levels.AddRange(replacementLevels);
        SpellcastingProgression = spellcasting.Value;
        return Result.Success();
    }
}

public sealed class ClassCoreTraits
{
    private ClassCoreTraits()
    {
        CharacterClassId = null!;
        HitDieId = null!;
    }

    private ClassCoreTraits(
        CompendiumEntityId id,
        CompendiumEntityId characterClassId,
        CompendiumEntityId hitDieId,
        CompendiumEntityId? armorTrainingCategoryId,
        int skillChoiceCount)
    {
        Id = id;
        CharacterClassId = characterClassId;
        HitDieId = hitDieId;
        ArmorTrainingCategoryId = armorTrainingCategoryId;
        SkillChoiceCount = skillChoiceCount;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId CharacterClassId { get; private set; }

    public CompendiumEntityId HitDieId { get; private set; }

    public CompendiumEntityId? ArmorTrainingCategoryId { get; private set; }

    public int SkillChoiceCount { get; private set; }

    public static ClassCoreTraits Create(
        CompendiumEntityId id,
        CompendiumEntityId characterClassId,
        CompendiumEntityId hitDieId,
        CompendiumEntityId? armorTrainingCategoryId,
        int skillChoiceCount) =>
        new(id, characterClassId, hitDieId, armorTrainingCategoryId, skillChoiceCount);

    public void Update(CompendiumEntityId hitDieId, CompendiumEntityId? armorTrainingCategoryId, int skillChoiceCount)
    {
        HitDieId = hitDieId;
        ArmorTrainingCategoryId = armorTrainingCategoryId;
        SkillChoiceCount = skillChoiceCount;
    }
}

public sealed class ClassPrimaryAbility
{
    private ClassPrimaryAbility()
    {
        CharacterClassId = null!;
        AbilityId = null!;
    }

    private ClassPrimaryAbility(CompendiumEntityId id, CompendiumEntityId characterClassId, CompendiumEntityId abilityId, int sortOrder)
    {
        Id = id;
        CharacterClassId = characterClassId;
        AbilityId = abilityId;
        SortOrder = sortOrder;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId CharacterClassId { get; private set; }

    public CompendiumEntityId AbilityId { get; private set; }

    public int SortOrder { get; private set; }

    public static ClassPrimaryAbility Create(
        CompendiumEntityId id,
        CompendiumEntityId characterClassId,
        CompendiumEntityId abilityId,
        int sortOrder) =>
        new(id, characterClassId, abilityId, sortOrder);
}

public sealed record ClassCoreTraitsInput(
    CompendiumEntityId HitDieId,
    CompendiumEntityId? ArmorTrainingCategoryId,
    int SkillChoiceCount);
