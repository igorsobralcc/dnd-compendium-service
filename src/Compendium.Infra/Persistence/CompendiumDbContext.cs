using Compendium.Domain.Classes;
using Compendium.Domain.Features;
using Compendium.Domain.Equipment;
using Compendium.Domain.Fundamentals;
using Compendium.Domain.Sources;
using Compendium.Domain.Translations;
using Compendium.Infra.Persistence.Classes;
using Compendium.Infra.Persistence.Features;
using Compendium.Infra.Persistence.Equipment;
using Compendium.Infra.Persistence.Fundamentals;
using Compendium.Infra.Persistence.Integration;
using Compendium.Infra.Persistence.Importing;
using Compendium.Infra.Persistence.Sources;
using Compendium.Infra.Persistence.Translations;
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

    public DbSet<CharacterClass> CharacterClasses => Set<CharacterClass>();

    public DbSet<ClassCoreTraits> ClassCoreTraits => Set<ClassCoreTraits>();

    public DbSet<ClassPrimaryAbility> ClassPrimaryAbilities => Set<ClassPrimaryAbility>();

    public DbSet<ClassLevel> ClassLevels => Set<ClassLevel>();

    public DbSet<ClassLevelSpellSlot> ClassLevelSpellSlots => Set<ClassLevelSpellSlot>();

    public DbSet<ClassProficiencyGrant> ClassProficiencyGrants => Set<ClassProficiencyGrant>();

    public DbSet<ClassWeaponMasteryCountByLevel> ClassWeaponMasteryCountsByLevel => Set<ClassWeaponMasteryCountByLevel>();

    public DbSet<ClassSpellcastingProgression> ClassSpellcastingProgressions => Set<ClassSpellcastingProgression>();

    public DbSet<ClassSpellcastingLevelRule> ClassSpellcastingLevelRules => Set<ClassSpellcastingLevelRule>();

    public DbSet<CharacterSubclass> CharacterSubclasses => Set<CharacterSubclass>();

    public DbSet<SubclassFeature> SubclassFeatures => Set<SubclassFeature>();

    public DbSet<Feature> Features => Set<Feature>();

    public DbSet<ClassLevelFeature> ClassLevelFeatures => Set<ClassLevelFeature>();

    public DbSet<SpeciesFeature> SpeciesFeatures => Set<SpeciesFeature>();

    public DbSet<BackgroundFeature> BackgroundFeatures => Set<BackgroundFeature>();

    public DbSet<FeatFeature> FeatFeatures => Set<FeatFeature>();

    public DbSet<EffectSchema> EffectSchemas => Set<EffectSchema>();

    public DbSet<EffectSchemaField> EffectSchemaFields => Set<EffectSchemaField>();

    public DbSet<FeatureEffect> FeatureEffects => Set<FeatureEffect>();

    public DbSet<FeatureEffectFieldValue> FeatureEffectFieldValues => Set<FeatureEffectFieldValue>();

    public DbSet<FeatureEffectCondition> FeatureEffectConditions => Set<FeatureEffectCondition>();

    public DbSet<EntityPrerequisite> EntityPrerequisites => Set<EntityPrerequisite>();

    public DbSet<ChoiceSet> ChoiceSets => Set<ChoiceSet>();

    public DbSet<ChoiceSetFilter> ChoiceSetFilters => Set<ChoiceSetFilter>();

    public DbSet<ChoiceOption> ChoiceOptions => Set<ChoiceOption>();
    public DbSet<EquipmentItem> EquipmentItems => Set<EquipmentItem>();
    public DbSet<Weapon> Weapons => Set<Weapon>();
    public DbSet<WeaponProperty> WeaponProperties => Set<WeaponProperty>();
    public DbSet<WeaponPropertyRule> WeaponPropertyRules => Set<WeaponPropertyRule>();
    public DbSet<WeaponPropertyLink> WeaponPropertyLinks => Set<WeaponPropertyLink>();
    public DbSet<WeaponPropertyLinkValue> WeaponPropertyLinkValues => Set<WeaponPropertyLinkValue>();
    public DbSet<WeaponMasteryProperty> WeaponMasteryProperties => Set<WeaponMasteryProperty>();
    public DbSet<WeaponMasteryEffect> WeaponMasteryEffects => Set<WeaponMasteryEffect>();
    public DbSet<WeaponMasteryRequirement> WeaponMasteryRequirements => Set<WeaponMasteryRequirement>();
    public DbSet<Armor> Armors => Set<Armor>();
    public DbSet<ArmorAcRule> ArmorAcRules => Set<ArmorAcRule>();
    public DbSet<ArmorDrawback> ArmorDrawbacks => Set<ArmorDrawback>();
    public DbSet<Tool> Tools => Set<Tool>();
    public DbSet<EquipmentPack> EquipmentPacks => Set<EquipmentPack>();
    public DbSet<EquipmentPackItem> EquipmentPackItems => Set<EquipmentPackItem>();
    public DbSet<StartingEquipmentRule> StartingEquipmentRules => Set<StartingEquipmentRule>();
    public DbSet<StartingEquipmentGroup> StartingEquipmentGroups => Set<StartingEquipmentGroup>();
    public DbSet<StartingEquipmentOption> StartingEquipmentOptions => Set<StartingEquipmentOption>();
    public DbSet<Translation> Translations => Set<Translation>();
    public DbSet<SourceVersionImportRecord> SourceVersionImports => Set<SourceVersionImportRecord>();
    public DbSet<SourceVersionValidationIssue> SourceVersionValidationIssues => Set<SourceVersionValidationIssue>();

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
        modelBuilder.ApplyConfiguration(new CharacterClassConfiguration());
        modelBuilder.ApplyConfiguration(new ClassCoreTraitsConfiguration());
        modelBuilder.ApplyConfiguration(new ClassPrimaryAbilityConfiguration());
        modelBuilder.ApplyConfiguration(new ClassLevelConfiguration());
        modelBuilder.ApplyConfiguration(new ClassLevelSpellSlotConfiguration());
        modelBuilder.ApplyConfiguration(new ClassProficiencyGrantConfiguration());
        modelBuilder.ApplyConfiguration(new ClassWeaponMasteryCountByLevelConfiguration());
        modelBuilder.ApplyConfiguration(new ClassSpellcastingProgressionConfiguration());
        modelBuilder.ApplyConfiguration(new ClassSpellcastingLevelRuleConfiguration());
        modelBuilder.ApplyConfiguration(new CharacterSubclassConfiguration());
        modelBuilder.ApplyConfiguration(new SubclassFeatureConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureLinkConfiguration<ClassLevelFeature>("class_level_features", "class_level_id"));
        modelBuilder.ApplyConfiguration(new FeatureLinkConfiguration<SpeciesFeature>("species_features", "species_id"));
        modelBuilder.ApplyConfiguration(new FeatureLinkConfiguration<BackgroundFeature>("background_features", "background_id"));
        modelBuilder.ApplyConfiguration(new FeatureLinkConfiguration<FeatFeature>("feat_features", "feat_id"));
        modelBuilder.ApplyConfiguration(new EffectSchemaConfiguration());
        modelBuilder.ApplyConfiguration(new EffectSchemaFieldConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureEffectConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureEffectFieldValueConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureEffectConditionConfiguration());
        modelBuilder.ApplyConfiguration(new EntityPrerequisiteConfiguration());
        modelBuilder.ApplyConfiguration(new ChoiceSetConfiguration());
        modelBuilder.ApplyConfiguration(new ChoiceSetFilterConfiguration());
        modelBuilder.ApplyConfiguration(new ChoiceOptionConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentItemConfiguration());
        modelBuilder.ApplyConfiguration(new WeaponConfiguration());
        modelBuilder.ApplyConfiguration(new WeaponPropertyConfiguration());
        modelBuilder.ApplyConfiguration(new WeaponPropertyRuleConfiguration());
        modelBuilder.ApplyConfiguration(new WeaponPropertyLinkConfiguration());
        modelBuilder.ApplyConfiguration(new WeaponPropertyLinkValueConfiguration());
        modelBuilder.ApplyConfiguration(new WeaponMasteryPropertyConfiguration());
        modelBuilder.ApplyConfiguration(new WeaponMasteryEffectConfiguration());
        modelBuilder.ApplyConfiguration(new WeaponMasteryRequirementConfiguration());
        modelBuilder.ApplyConfiguration(new ArmorConfiguration());
        modelBuilder.ApplyConfiguration(new ArmorAcRuleConfiguration());
        modelBuilder.ApplyConfiguration(new ArmorDrawbackConfiguration());
        modelBuilder.ApplyConfiguration(new ToolConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentPackConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentPackItemConfiguration());
        modelBuilder.ApplyConfiguration(new StartingEquipmentRuleConfiguration());
        modelBuilder.ApplyConfiguration(new StartingEquipmentGroupConfiguration());
        modelBuilder.ApplyConfiguration(new StartingEquipmentOptionConfiguration());
        modelBuilder.ApplyConfiguration(new TranslationConfiguration());
        modelBuilder.ApplyConfiguration(new SourceVersionImportRecordConfiguration());
        modelBuilder.ApplyConfiguration(new SourceVersionValidationIssueConfiguration());
    }
}
