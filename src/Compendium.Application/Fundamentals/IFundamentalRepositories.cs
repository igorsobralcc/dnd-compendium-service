using Compendium.Domain.Fundamentals;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Fundamentals;

public interface IAbilityRepository
{
    Task AddAsync(Ability ability, CancellationToken cancellationToken);

    Task<Ability?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken);

    Task<Ability?> GetByCodeAsync(AbilityCode code, CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(AbilityCode code, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Ability>> ListAsync(CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface ISkillRepository
{
    Task AddAsync(Skill skill, CancellationToken cancellationToken);

    Task<Skill?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken);

    Task<Skill?> GetByCodeAsync(SkillCode code, CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(SkillCode code, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Skill>> ListAsync(CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface ILanguageRepository
{
    Task AddAsync(Language language, CancellationToken cancellationToken);

    Task<Language?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken);

    Task<Language?> GetByCodeAsync(LanguageCode code, CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(LanguageCode code, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Language>> ListAsync(CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IProficiencyRepository
{
    Task AddAsync(Proficiency proficiency, CancellationToken cancellationToken);

    Task<Proficiency?> GetByCodeAsync(ProficiencyCode code, CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(ProficiencyCode code, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Proficiency>> ListAsync(ProficiencyType? type, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IArmorTrainingCategoryRepository
{
    Task AddAsync(ArmorTrainingCategory category, CancellationToken cancellationToken);

    Task<ArmorTrainingCategory?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(ArmorTrainingCategoryCode code, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ArmorTrainingCategory>> ListAsync(CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IHitDieRepository
{
    Task AddAsync(HitDie hitDie, CancellationToken cancellationToken);

    Task<bool> ExistsByDieAsync(int die, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<HitDie>> ListAsync(CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IAbilityScoreMethodRepository
{
    Task AddAsync(AbilityScoreMethod method, CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(AbilityScoreMethodCode code, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AbilityScoreMethod>> ListAsync(CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
