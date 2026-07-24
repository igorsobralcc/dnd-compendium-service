using Compendium.Application.Errors;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Features;

public static class FeatureErrors
{
    public static ApplicationError FeatureCodeAlreadyExists(string code) =>
        new("compendium.features.code-conflict", $"A feature with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError FeatureNotFound(string code) =>
        new("compendium.features.not-found", $"Feature '{code}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError EffectSchemaCodeAlreadyExists(string code) =>
        new("compendium.features.effect-schema.code-conflict", $"An effect schema with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError EffectSchemaNotFound(string code) =>
        new("compendium.features.effect-schema.not-found", $"Effect schema '{code}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError PrerequisiteNotFound(string id) =>
        new("compendium.features.prerequisite.not-found", $"Prerequisite '{id}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError ChoiceSetCodeAlreadyExists(string code) =>
        new("compendium.features.choice-set.code-conflict", $"A choice set with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError ChoiceSetNotFound(string code) =>
        new("compendium.features.choice-set.not-found", $"Choice set '{code}' was not found.", ApplicationErrorKind.NotFound);

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
