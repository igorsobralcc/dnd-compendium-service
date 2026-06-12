using Compendium.Application.Errors;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Sources;

public static class SourceErrors
{
    public static ApplicationError RulesetCodeAlreadyExists(string code) =>
        new("compendium.rulesets.code-conflict", $"A ruleset with code '{code}' already exists.", ApplicationErrorKind.Conflict);

    public static ApplicationError RulesetNotFound(string codeOrId) =>
        new("compendium.rulesets.not-found", $"Ruleset '{codeOrId}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError RuleSourceCodeAlreadyExists(string code) =>
        new("compendium.rule-sources.code-conflict", $"A rule source with code '{code}' already exists for this ruleset.", ApplicationErrorKind.Conflict);

    public static ApplicationError RuleSourceNotFound(string id) =>
        new("compendium.rule-sources.not-found", $"Rule source '{id}' was not found.", ApplicationErrorKind.NotFound);

    public static ApplicationError SourceVersionAlreadyExists(string number) =>
        new("compendium.source-versions.version-conflict", $"Source version '{number}' already exists for this source.", ApplicationErrorKind.Conflict);

    public static ApplicationError CurrentSourceVersionNotFound(string sourceId) =>
        new("compendium.source-versions.current-not-found", $"Current source version for source '{sourceId}' was not found.", ApplicationErrorKind.NotFound);

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
