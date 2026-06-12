using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Classes;

public sealed class ClassLevel
{
    private readonly List<ClassLevelSpellSlot> spellSlots = [];
    private readonly List<ClassProficiencyGrant> proficiencyGrants = [];
    private readonly List<ClassWeaponMasteryCountByLevel> weaponMasteryCounts = [];

    private ClassLevel()
    {
        CharacterClassId = null!;
    }

    private ClassLevel(CompendiumEntityId id, CompendiumEntityId characterClassId, int level, int? proficiencyBonus)
    {
        Id = id;
        CharacterClassId = characterClassId;
        Level = level;
        ProficiencyBonus = proficiencyBonus;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId CharacterClassId { get; private set; }

    public int Level { get; private set; }

    public int? ProficiencyBonus { get; private set; }

    public IReadOnlyCollection<ClassLevelSpellSlot> SpellSlots => spellSlots;

    public IReadOnlyCollection<ClassProficiencyGrant> ProficiencyGrants => proficiencyGrants;

    public IReadOnlyCollection<ClassWeaponMasteryCountByLevel> WeaponMasteryCounts => weaponMasteryCounts;

    public static Result<ClassLevel> Create(
        CompendiumEntityId id,
        CompendiumEntityId characterClassId,
        int level,
        int? proficiencyBonus,
        IReadOnlyCollection<ClassLevelSpellSlotInput> spellSlotInputs,
        IReadOnlyCollection<CompendiumEntityId> proficiencyGrantIds,
        int? weaponMasteryCount)
    {
        if (proficiencyBonus.HasValue && proficiencyBonus.Value <= 0)
        {
            return Result<ClassLevel>.Failure(ClassDomainErrors.InvalidProficiencyBonus(level));
        }

        var classLevel = new ClassLevel(id, characterClassId, level, proficiencyBonus);

        var spellSlotsResult = classLevel.AddSpellSlots(spellSlotInputs);
        if (spellSlotsResult.IsFailure) return Result<ClassLevel>.Failure(spellSlotsResult.Error);

        var grantsResult = classLevel.AddProficiencyGrants(proficiencyGrantIds);
        if (grantsResult.IsFailure) return Result<ClassLevel>.Failure(grantsResult.Error);

        var weaponMasteryResult = classLevel.SetWeaponMasteryCount(weaponMasteryCount);
        return weaponMasteryResult.IsFailure
            ? Result<ClassLevel>.Failure(weaponMasteryResult.Error)
            : Result<ClassLevel>.Success(classLevel);
    }

    private Result AddSpellSlots(IReadOnlyCollection<ClassLevelSpellSlotInput> spellSlotInputs)
    {
        var seenSpellLevels = new HashSet<int>();
        foreach (var input in spellSlotInputs.OrderBy(input => input.SpellLevel))
        {
            if (input.SpellLevel is < 1 or > 9)
            {
                return Result.Failure(ClassDomainErrors.InvalidSpellSlotLevel(input.SpellLevel));
            }

            if (input.Slots <= 0)
            {
                return Result.Failure(ClassDomainErrors.InvalidSpellSlotCount(input.SpellLevel));
            }

            if (!seenSpellLevels.Add(input.SpellLevel))
            {
                return Result.Failure(ClassDomainErrors.DuplicateSpellSlotLevel(Level, input.SpellLevel));
            }

            spellSlots.Add(ClassLevelSpellSlot.Create(CompendiumEntityId.New(), Id, input.SpellLevel, input.Slots));
        }

        return Result.Success();
    }

    private Result AddProficiencyGrants(IReadOnlyCollection<CompendiumEntityId> proficiencyIds)
    {
        var seenProficiencyIds = new HashSet<CompendiumEntityId>();
        foreach (var proficiencyId in proficiencyIds)
        {
            if (!seenProficiencyIds.Add(proficiencyId))
            {
                return Result.Failure(ClassDomainErrors.DuplicateProficiencyGrant(Level, proficiencyId.ToString()));
            }

            proficiencyGrants.Add(ClassProficiencyGrant.Create(CompendiumEntityId.New(), Id, proficiencyId));
        }

        return Result.Success();
    }

    private Result SetWeaponMasteryCount(int? count)
    {
        if (!count.HasValue)
        {
            return Result.Success();
        }

        if (count.Value < 0)
        {
            return Result.Failure(ClassDomainErrors.InvalidWeaponMasteryCount(Level));
        }

        weaponMasteryCounts.Add(ClassWeaponMasteryCountByLevel.Create(CompendiumEntityId.New(), Id, count.Value));
        return Result.Success();
    }
}

public sealed class ClassLevelSpellSlot
{
    private ClassLevelSpellSlot()
    {
        ClassLevelId = null!;
    }

    private ClassLevelSpellSlot(CompendiumEntityId id, CompendiumEntityId classLevelId, int spellLevel, int slots)
    {
        Id = id;
        ClassLevelId = classLevelId;
        SpellLevel = spellLevel;
        Slots = slots;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId ClassLevelId { get; private set; }

    public int SpellLevel { get; private set; }

    public int Slots { get; private set; }

    public static ClassLevelSpellSlot Create(CompendiumEntityId id, CompendiumEntityId classLevelId, int spellLevel, int slots) =>
        new(id, classLevelId, spellLevel, slots);
}

public sealed class ClassProficiencyGrant
{
    private ClassProficiencyGrant()
    {
        ClassLevelId = null!;
        ProficiencyId = null!;
    }

    private ClassProficiencyGrant(CompendiumEntityId id, CompendiumEntityId classLevelId, CompendiumEntityId proficiencyId)
    {
        Id = id;
        ClassLevelId = classLevelId;
        ProficiencyId = proficiencyId;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId ClassLevelId { get; private set; }

    public CompendiumEntityId ProficiencyId { get; private set; }

    public static ClassProficiencyGrant Create(CompendiumEntityId id, CompendiumEntityId classLevelId, CompendiumEntityId proficiencyId) =>
        new(id, classLevelId, proficiencyId);
}

public sealed class ClassWeaponMasteryCountByLevel
{
    private ClassWeaponMasteryCountByLevel()
    {
        ClassLevelId = null!;
    }

    private ClassWeaponMasteryCountByLevel(CompendiumEntityId id, CompendiumEntityId classLevelId, int count)
    {
        Id = id;
        ClassLevelId = classLevelId;
        Count = count;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId ClassLevelId { get; private set; }

    public int Count { get; private set; }

    public static ClassWeaponMasteryCountByLevel Create(CompendiumEntityId id, CompendiumEntityId classLevelId, int count) =>
        new(id, classLevelId, count);
}

public sealed class ClassSpellcastingProgression
{
    private readonly List<ClassSpellcastingLevelRule> levelRules = [];

    private ClassSpellcastingProgression()
    {
        CharacterClassId = null!;
    }

    private ClassSpellcastingProgression(
        CompendiumEntityId id,
        CompendiumEntityId characterClassId,
        ClassSpellcastingProgressionType type,
        CompendiumEntityId? spellcastingAbilityId)
    {
        Id = id;
        CharacterClassId = characterClassId;
        Type = type;
        SpellcastingAbilityId = spellcastingAbilityId;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId CharacterClassId { get; private set; }

    public ClassSpellcastingProgressionType Type { get; private set; }

    public CompendiumEntityId? SpellcastingAbilityId { get; private set; }

    public IReadOnlyCollection<ClassSpellcastingLevelRule> LevelRules => levelRules;

    public static Result<ClassSpellcastingProgression?> Create(
        CompendiumEntityId id,
        CompendiumEntityId characterClassId,
        ClassSpellcastingProgressionInput input)
    {
        if (!Enum.IsDefined(input.Type))
        {
            return Result<ClassSpellcastingProgression?>.Failure(ClassDomainErrors.InvalidSpellcastingProgressionType());
        }

        var progression = new ClassSpellcastingProgression(id, characterClassId, input.Type, input.SpellcastingAbilityId);
        var seenClassLevels = new HashSet<int>();
        foreach (var rule in input.LevelRules.OrderBy(rule => rule.ClassLevel))
        {
            if (!CharacterClass.IsSupportedLevel(rule.ClassLevel) || rule.CasterLevel <= 0)
            {
                return Result<ClassSpellcastingProgression?>.Failure(ClassDomainErrors.InvalidSpellcastingLevelRule(rule.ClassLevel));
            }

            if (!seenClassLevels.Add(rule.ClassLevel))
            {
                return Result<ClassSpellcastingProgression?>.Failure(ClassDomainErrors.DuplicateSpellcastingLevelRule(rule.ClassLevel));
            }

            progression.levelRules.Add(ClassSpellcastingLevelRule.Create(
                CompendiumEntityId.New(),
                progression.Id,
                rule.ClassLevel,
                rule.CasterLevel));
        }

        return Result<ClassSpellcastingProgression?>.Success(progression);
    }
}

public sealed class ClassSpellcastingLevelRule
{
    private ClassSpellcastingLevelRule()
    {
        ClassSpellcastingProgressionId = null!;
    }

    private ClassSpellcastingLevelRule(
        CompendiumEntityId id,
        CompendiumEntityId classSpellcastingProgressionId,
        int classLevel,
        int casterLevel)
    {
        Id = id;
        ClassSpellcastingProgressionId = classSpellcastingProgressionId;
        ClassLevel = classLevel;
        CasterLevel = casterLevel;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId ClassSpellcastingProgressionId { get; private set; }

    public int ClassLevel { get; private set; }

    public int CasterLevel { get; private set; }

    public static ClassSpellcastingLevelRule Create(
        CompendiumEntityId id,
        CompendiumEntityId classSpellcastingProgressionId,
        int classLevel,
        int casterLevel) =>
        new(id, classSpellcastingProgressionId, classLevel, casterLevel);
}

public sealed record ClassLevelInput(
    int Level,
    int? ProficiencyBonus,
    IReadOnlyCollection<ClassLevelSpellSlotInput> SpellSlots,
    IReadOnlyCollection<CompendiumEntityId> ProficiencyGrantIds,
    int? WeaponMasteryCount);

public sealed record ClassLevelSpellSlotInput(int SpellLevel, int Slots);

public sealed record ClassSpellcastingProgressionInput(
    ClassSpellcastingProgressionType Type,
    CompendiumEntityId? SpellcastingAbilityId,
    IReadOnlyCollection<ClassSpellcastingLevelRuleInput> LevelRules);

public sealed record ClassSpellcastingLevelRuleInput(int ClassLevel, int CasterLevel);
