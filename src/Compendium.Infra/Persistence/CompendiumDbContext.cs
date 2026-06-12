using Compendium.Domain.Fundamentals;
using Compendium.Domain.Sources;
using Compendium.Infra.Persistence.Fundamentals;
using Compendium.Infra.Persistence.Integration;
using Compendium.Infra.Persistence.Sources;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence;

public sealed class CompendiumDbContext : DbContext
{
    public const string Schema = "compendium";
    public const string MigrationsHistoryTable = "__ef_migrations_history";

    public CompendiumDbContext(DbContextOptions<CompendiumDbContext> options)
        : base(options)
    {
    }

    public DbSet<IntegrationOutbox> IntegrationOutbox => Set<IntegrationOutbox>();

    public DbSet<IntegrationOutboxField> IntegrationOutboxFields => Set<IntegrationOutboxField>();

    public DbSet<IntegrationInbox> IntegrationInbox => Set<IntegrationInbox>();

    public DbSet<Ruleset> Rulesets => Set<Ruleset>();

    public DbSet<RuleSource> RuleSources => Set<RuleSource>();

    public DbSet<SourceVersion> SourceVersions => Set<SourceVersion>();

    public DbSet<Ability> Abilities => Set<Ability>();

    public DbSet<Skill> Skills => Set<Skill>();

    public DbSet<Language> Languages => Set<Language>();

    public DbSet<Proficiency> Proficiencies => Set<Proficiency>();

    public DbSet<ArmorTrainingCategory> ArmorTrainingCategories => Set<ArmorTrainingCategory>();

    public DbSet<HitDie> HitDice => Set<HitDie>();

    public DbSet<AbilityScoreMethod> AbilityScoreMethods => Set<AbilityScoreMethod>();

    public DbSet<AbilityScoreMethodRule> AbilityScoreMethodRules => Set<AbilityScoreMethodRule>();

    public DbSet<AbilityScoreStandardValue> AbilityScoreStandardValues => Set<AbilityScoreStandardValue>();

    public DbSet<AbilityScorePointBuyCost> AbilityScorePointBuyCosts => Set<AbilityScorePointBuyCost>();

    public DbSet<AbilityScoreRollRule> AbilityScoreRollRules => Set<AbilityScoreRollRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfiguration(new IntegrationOutboxConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationOutboxFieldConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationInboxConfiguration());
        modelBuilder.ApplyConfiguration(new RulesetConfiguration());
        modelBuilder.ApplyConfiguration(new RuleSourceConfiguration());
        modelBuilder.ApplyConfiguration(new SourceVersionConfiguration());
        modelBuilder.ApplyConfiguration(new AbilityConfiguration());
        modelBuilder.ApplyConfiguration(new SkillConfiguration());
        modelBuilder.ApplyConfiguration(new LanguageConfiguration());
        modelBuilder.ApplyConfiguration(new ProficiencyConfiguration());
        modelBuilder.ApplyConfiguration(new ArmorTrainingCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new HitDieConfiguration());
        modelBuilder.ApplyConfiguration(new AbilityScoreMethodConfiguration());
        modelBuilder.ApplyConfiguration(new AbilityScoreMethodRuleConfiguration());
        modelBuilder.ApplyConfiguration(new AbilityScoreStandardValueConfiguration());
        modelBuilder.ApplyConfiguration(new AbilityScorePointBuyCostConfiguration());
        modelBuilder.ApplyConfiguration(new AbilityScoreRollRuleConfiguration());
    }
}
