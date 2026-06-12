using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Sources;

public static class SourceDomainErrors
{
    public static DomainError Required(string field) =>
        new($"compendium.sources.{field}.required", $"The {field} is required.");

    public static DomainError TooLong(string field, int maxLength) =>
        new($"compendium.sources.{field}.too-long", $"The {field} must have at most {maxLength} characters.");

    public static DomainError InvalidCode(string field) =>
        new($"compendium.sources.{field}.invalid", $"The {field} must contain only uppercase letters, numbers, dot, underscore or hyphen.");

    public static DomainError InvalidStatus(string field) =>
        new($"compendium.sources.{field}.invalid", $"The {field} is not supported.");

    public static DomainError InvalidSourceVersionPublicationDate() =>
        new("compendium.sources.publication-date.invalid", "The publication date cannot be in the future.");
}
