namespace Compendium.Domain.Importing;

public enum ValidationIssueSeverity
{
    Blocker = 1,
    Warning = 2,
    Info = 3
}

public sealed record SourceVersionContentSummary(
    int AbilityCount,
    int SkillCount,
    int LanguageCount,
    int ProficiencyCount,
    int HitDieCount,
    int ClassCount,
    int ClassLevelCount,
    int EquipmentCount,
    int SpeciesCount,
    int BackgroundCount,
    int FeatCount,
    int SpellCount,
    int InvalidFeatureEffectCount,
    int InvalidSpellListEntryCount,
    int InvalidBackgroundGrantCount,
    int InvalidEquipmentReferenceCount);

public sealed record ConsistencyIssue(string Code, ValidationIssueSeverity Severity, string Message);

public interface ICompendiumConsistencySpecification
{
    IEnumerable<ConsistencyIssue> Evaluate(SourceVersionContentSummary summary);
}

public sealed class CompendiumConsistencyChecker
{
    private readonly IReadOnlyCollection<ICompendiumConsistencySpecification> specifications;

    public CompendiumConsistencyChecker()
        : this(
        [
            new FundamentalContentMustExistSpecification(),
            new ClassMustHaveLevelProgressionSpecification(),
            new FeatureEffectsMustMatchSchemaSpecification(),
            new SpellMustHaveValidListEntriesSpecification(),
            new BackgroundMustHaveValidGrantsSpecification(),
            new EquipmentReferencesMustBeValidSpecification()
        ])
    {
    }

    public CompendiumConsistencyChecker(IEnumerable<ICompendiumConsistencySpecification> specifications) =>
        this.specifications = specifications.ToArray();

    public IReadOnlyCollection<ConsistencyIssue> Check(SourceVersionContentSummary summary) =>
        specifications.SelectMany(specification => specification.Evaluate(summary)).ToArray();
}

public sealed class FundamentalContentMustExistSpecification : ICompendiumConsistencySpecification
{
    public IEnumerable<ConsistencyIssue> Evaluate(SourceVersionContentSummary summary)
    {
        if (summary.AbilityCount == 0) yield return Missing("abilities");
        if (summary.SkillCount == 0) yield return Missing("skills");
        if (summary.LanguageCount == 0) yield return Missing("languages");
        if (summary.ProficiencyCount == 0) yield return Missing("proficiencies");
        if (summary.HitDieCount == 0) yield return Missing("hit-dice");
        if (summary.EquipmentCount == 0) yield return Missing("equipment");
    }

    private static ConsistencyIssue Missing(string category) =>
        new($"MISSING_{category.Replace("-", "_").ToUpperInvariant()}", ValidationIssueSeverity.Blocker, $"Source version has no {category}.");
}

public sealed class ClassMustHaveLevelProgressionSpecification : ICompendiumConsistencySpecification
{
    public IEnumerable<ConsistencyIssue> Evaluate(SourceVersionContentSummary summary)
    {
        if (summary.ClassCount == 0)
            yield return new("MISSING_CLASSES", ValidationIssueSeverity.Blocker, "Source version has no classes.");
        else if (summary.ClassLevelCount < summary.ClassCount)
            yield return new("CLASS_WITHOUT_LEVEL", ValidationIssueSeverity.Blocker, "Every class must have at least one level.");
    }
}

public sealed class FeatureEffectsMustMatchSchemaSpecification : ICompendiumConsistencySpecification
{
    public IEnumerable<ConsistencyIssue> Evaluate(SourceVersionContentSummary summary)
    {
        if (summary.InvalidFeatureEffectCount > 0)
            yield return new("INVALID_FEATURE_EFFECT", ValidationIssueSeverity.Blocker, $"{summary.InvalidFeatureEffectCount} feature effect(s) do not match their schema.");
    }
}

public sealed class SpellMustHaveValidListEntriesSpecification : ICompendiumConsistencySpecification
{
    public IEnumerable<ConsistencyIssue> Evaluate(SourceVersionContentSummary summary)
    {
        if (summary.SpellCount == 0)
            yield return new("MISSING_SPELLS", ValidationIssueSeverity.Warning, "Source version has no spells.");
        if (summary.InvalidSpellListEntryCount > 0)
            yield return new("INVALID_SPELL_LIST_ENTRY", ValidationIssueSeverity.Blocker, $"{summary.InvalidSpellListEntryCount} spell-list entry reference(s) are invalid.");
    }
}

public sealed class BackgroundMustHaveValidGrantsSpecification : ICompendiumConsistencySpecification
{
    public IEnumerable<ConsistencyIssue> Evaluate(SourceVersionContentSummary summary)
    {
        if (summary.BackgroundCount == 0)
            yield return new("MISSING_BACKGROUNDS", ValidationIssueSeverity.Warning, "Source version has no backgrounds.");
        if (summary.InvalidBackgroundGrantCount > 0)
            yield return new("INVALID_BACKGROUND_GRANT", ValidationIssueSeverity.Blocker, $"{summary.InvalidBackgroundGrantCount} background grant reference(s) are invalid.");
    }
}

public sealed class EquipmentReferencesMustBeValidSpecification : ICompendiumConsistencySpecification
{
    public IEnumerable<ConsistencyIssue> Evaluate(SourceVersionContentSummary summary)
    {
        if (summary.InvalidEquipmentReferenceCount > 0)
            yield return new("INVALID_EQUIPMENT_REFERENCE", ValidationIssueSeverity.Blocker, $"{summary.InvalidEquipmentReferenceCount} equipment reference(s) are invalid.");
    }
}
