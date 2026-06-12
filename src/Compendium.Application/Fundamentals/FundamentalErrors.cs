using Compendium.Application.Errors;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Fundamentals;

public static class FundamentalErrors
{
    public static ApplicationError AbilityCodeAlreadyExists(string code) =>
        new("compendium.abilities.code-conflict", $"An ability with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError AbilityNotFound(string codeOrId) =>
        new("compendium.abilities.not-found", $"Ability '{codeOrId}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError SkillCodeAlreadyExists(string code) =>
        new("compendium.skills.code-conflict", $"A skill with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError SkillNotFound(string codeOrId) =>
        new("compendium.skills.not-found", $"Skill '{codeOrId}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError LanguageCodeAlreadyExists(string code) =>
        new("compendium.languages.code-conflict", $"A language with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError LanguageNotFound(string codeOrId) =>
        new("compendium.languages.not-found", $"Language '{codeOrId}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError ProficiencyCodeAlreadyExists(string code) =>
        new("compendium.proficiencies.code-conflict", $"A proficiency with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError ProficiencyNotFound(string code) =>
        new("compendium.proficiencies.not-found", $"Proficiency '{code}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError ArmorTrainingCategoryCodeAlreadyExists(string code) =>
        new("compendium.armor-training-categories.code-conflict", $"An armor training category with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError ArmorTrainingCategoryNotFound(string id) =>
        new("compendium.armor-training-categories.not-found", $"Armor training category '{id}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError HitDieAlreadyExists(int die) =>
        new("compendium.hit-dice.die-conflict", $"Hit die d{die} already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError AbilityScoreMethodCodeAlreadyExists(string code) =>
        new("compendium.ability-score-methods.code-conflict", $"An ability score method with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError SourceVersionNotFound(string id) =>
        new("compendium.source-versions.not-found", $"Source version '{id}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError SourceVersionDoesNotBelongToSource(string sourceVersionId, string sourceId) =>
        new("compendium.source-versions.source-mismatch", $"Source version '{sourceVersionId}' does not belong to rule source '{sourceId}'.", ApplicationErrorKind.Validation);

    public static ApplicationError RelatedEntityNotFound(string id) =>
        new("compendium.proficiencies.related-entity-not-found", $"Related entity '{id}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError FromDomain(DomainError error) =>
        new(error.Code, error.Message, ToKind(error.Kind));

    private static ApplicationErrorKind ToKind(DomainErrorKind kind) =>
        kind switch
        {
            DomainErrorKind.Validation => ApplicationErrorKind.Validation,
            DomainErrorKind.Conflict => ApplicationErrorKind.Conflict,
            DomainErrorKind.NotFound => ApplicationErrorKind.NotFound,
            DomainErrorKind.Unauthorized => ApplicationErrorKind.Unauthorized,
            DomainErrorKind.Forbidden => ApplicationErrorKind.Forbidden,
            DomainErrorKind.Unexpected => ApplicationErrorKind.Unexpected,
            _ => ApplicationErrorKind.Unexpected
        };
}
