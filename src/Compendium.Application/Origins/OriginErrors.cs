using Compendium.Application.Errors;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Origins;

public static class OriginErrors
{
    public static ApplicationError CodeAlreadyExists(string entity, string code) =>
        new($"compendium.origins.{entity}.code-conflict", $"A {entity} with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError NotFound(string entity, string value) =>
        new($"compendium.origins.{entity}.not-found", $"{entity} '{value}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError ReferenceNotFound(string entity, string id) =>
        new($"compendium.origins.{entity}.not-found", $"Referenced {entity} '{id}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError SourceVersionMismatch() =>
        new("compendium.origins.source-version.mismatch", "The source version does not belong to the supplied rule source.", ApplicationErrorKind.Validation);

    public static ApplicationError FromDomain(DomainError error) =>
        new(error.Code, error.Message, error.Kind switch
        {
            DomainErrorKind.Validation => ApplicationErrorKind.Validation,
            DomainErrorKind.Conflict => ApplicationErrorKind.Conflict,
            DomainErrorKind.NotFound => ApplicationErrorKind.NotFound,
            DomainErrorKind.Unauthorized => ApplicationErrorKind.Unauthorized,
            DomainErrorKind.Forbidden => ApplicationErrorKind.Forbidden,
            _ => ApplicationErrorKind.Unexpected
        });
}
