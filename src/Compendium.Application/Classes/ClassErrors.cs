using Compendium.Application.Errors;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Classes;

public static class ClassErrors
{
    public static ApplicationError ClassCodeAlreadyExists(string code) =>
        new("compendium.classes.code-conflict", $"A class with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError ClassNotFound(string code) =>
        new("compendium.classes.not-found", $"Class '{code}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError SubclassCodeAlreadyExists(string classCode, string subclassCode) =>
        new("compendium.classes.subclasses.code-conflict", $"Subclass '{subclassCode}' already exists for class '{classCode}'.", ApplicationErrorKind.Conflict);

    public static ApplicationError SubclassNotFound(string classCode, string subclassCode) =>
        new("compendium.classes.subclasses.not-found", $"Subclass '{subclassCode}' for class '{classCode}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError HitDieNotFound(string id) =>
        new("compendium.classes.hit-die.not-found", $"Hit die '{id}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError ArmorTrainingCategoryNotFound(string id) =>
        new("compendium.classes.armor-training-category.not-found", $"Armor training category '{id}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError AbilityNotFound(string id) =>
        new("compendium.classes.ability.not-found", $"Ability '{id}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError ProficiencyNotFound(string id) =>
        new("compendium.classes.proficiency.not-found", $"Proficiency '{id}' was not found.", ApplicationErrorKind.NotFound);

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
