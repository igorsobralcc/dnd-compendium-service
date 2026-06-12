using Compendium.Application.Fundamentals;
using Compendium.Domain.Fundamentals;
using Compendium.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence.Fundamentals;

internal sealed class AbilityRepository : IAbilityRepository
{
    private readonly CompendiumDbContext dbContext;

    public AbilityRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(Ability ability, CancellationToken cancellationToken) =>
        await dbContext.Abilities.AddAsync(ability, cancellationToken);

    public Task<Ability?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken) =>
        dbContext.Abilities.SingleOrDefaultAsync(ability => ability.Id == id, cancellationToken);

    public Task<Ability?> GetByCodeAsync(AbilityCode code, CancellationToken cancellationToken) =>
        dbContext.Abilities.SingleOrDefaultAsync(ability => ability.Code == code, cancellationToken);

    public Task<bool> ExistsByCodeAsync(AbilityCode code, CancellationToken cancellationToken) =>
        dbContext.Abilities.AnyAsync(ability => ability.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<Ability>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.Abilities.OrderBy(ability => ability.Code).ToArrayAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}

internal sealed class SkillRepository : ISkillRepository
{
    private readonly CompendiumDbContext dbContext;

    public SkillRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(Skill skill, CancellationToken cancellationToken) =>
        await dbContext.Skills.AddAsync(skill, cancellationToken);

    public Task<Skill?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken) =>
        dbContext.Skills.SingleOrDefaultAsync(skill => skill.Id == id, cancellationToken);

    public Task<Skill?> GetByCodeAsync(SkillCode code, CancellationToken cancellationToken) =>
        dbContext.Skills.SingleOrDefaultAsync(skill => skill.Code == code, cancellationToken);

    public Task<bool> ExistsByCodeAsync(SkillCode code, CancellationToken cancellationToken) =>
        dbContext.Skills.AnyAsync(skill => skill.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<Skill>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.Skills.OrderBy(skill => skill.Code).ToArrayAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}

internal sealed class LanguageRepository : ILanguageRepository
{
    private readonly CompendiumDbContext dbContext;

    public LanguageRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(Language language, CancellationToken cancellationToken) =>
        await dbContext.Languages.AddAsync(language, cancellationToken);

    public Task<Language?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken) =>
        dbContext.Languages.SingleOrDefaultAsync(language => language.Id == id, cancellationToken);

    public Task<Language?> GetByCodeAsync(LanguageCode code, CancellationToken cancellationToken) =>
        dbContext.Languages.SingleOrDefaultAsync(language => language.Code == code, cancellationToken);

    public Task<bool> ExistsByCodeAsync(LanguageCode code, CancellationToken cancellationToken) =>
        dbContext.Languages.AnyAsync(language => language.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<Language>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.Languages.OrderBy(language => language.Code).ToArrayAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}

internal sealed class ProficiencyRepository : IProficiencyRepository
{
    private readonly CompendiumDbContext dbContext;

    public ProficiencyRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(Proficiency proficiency, CancellationToken cancellationToken) =>
        await dbContext.Proficiencies.AddAsync(proficiency, cancellationToken);

    public Task<Proficiency?> GetByCodeAsync(ProficiencyCode code, CancellationToken cancellationToken) =>
        dbContext.Proficiencies.SingleOrDefaultAsync(proficiency => proficiency.Code == code, cancellationToken);

    public Task<bool> ExistsByCodeAsync(ProficiencyCode code, CancellationToken cancellationToken) =>
        dbContext.Proficiencies.AnyAsync(proficiency => proficiency.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<Proficiency>> ListAsync(ProficiencyType? type, CancellationToken cancellationToken)
    {
        var query = dbContext.Proficiencies.AsQueryable();
        if (type.HasValue)
        {
            query = query.Where(proficiency => proficiency.Type == type.Value);
        }

        return await query
            .OrderBy(proficiency => proficiency.Type)
            .ThenBy(proficiency => proficiency.Code)
            .ToArrayAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}

internal sealed class ArmorTrainingCategoryRepository : IArmorTrainingCategoryRepository
{
    private readonly CompendiumDbContext dbContext;

    public ArmorTrainingCategoryRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(ArmorTrainingCategory category, CancellationToken cancellationToken) =>
        await dbContext.ArmorTrainingCategories.AddAsync(category, cancellationToken);

    public Task<ArmorTrainingCategory?> GetByIdAsync(CompendiumEntityId id, CancellationToken cancellationToken) =>
        dbContext.ArmorTrainingCategories.SingleOrDefaultAsync(category => category.Id == id, cancellationToken);

    public Task<bool> ExistsByCodeAsync(ArmorTrainingCategoryCode code, CancellationToken cancellationToken) =>
        dbContext.ArmorTrainingCategories.AnyAsync(category => category.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<ArmorTrainingCategory>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.ArmorTrainingCategories
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Code)
            .ToArrayAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}

internal sealed class HitDieRepository : IHitDieRepository
{
    private readonly CompendiumDbContext dbContext;

    public HitDieRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(HitDie hitDie, CancellationToken cancellationToken) =>
        await dbContext.HitDice.AddAsync(hitDie, cancellationToken);

    public Task<bool> ExistsByDieAsync(int die, CancellationToken cancellationToken) =>
        dbContext.HitDice.AnyAsync(hitDie => hitDie.Die == die, cancellationToken);

    public async Task<IReadOnlyCollection<HitDie>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.HitDice.OrderBy(hitDie => hitDie.Die).ToArrayAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}

internal sealed class AbilityScoreMethodRepository : IAbilityScoreMethodRepository
{
    private readonly CompendiumDbContext dbContext;

    public AbilityScoreMethodRepository(CompendiumDbContext dbContext) => this.dbContext = dbContext;

    public async Task AddAsync(AbilityScoreMethod method, CancellationToken cancellationToken) =>
        await dbContext.AbilityScoreMethods.AddAsync(method, cancellationToken);

    public Task<bool> ExistsByCodeAsync(AbilityScoreMethodCode code, CancellationToken cancellationToken) =>
        dbContext.AbilityScoreMethods.AnyAsync(method => method.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<AbilityScoreMethod>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.AbilityScoreMethods
            .Include(method => method.Rules)
            .Include(method => method.StandardValues)
            .Include(method => method.PointBuyCosts)
            .Include(method => method.RollRules)
            .OrderBy(method => method.Type)
            .ThenBy(method => method.Code)
            .ToArrayAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
