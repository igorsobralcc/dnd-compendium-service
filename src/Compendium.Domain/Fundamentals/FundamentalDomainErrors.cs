using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Fundamentals;

public static class FundamentalDomainErrors
{
    public static DomainError Required(string field) =>
        new($"compendium.fundamentals.{field}.required", $"The {field} is required.");

    public static DomainError TooLong(string field, int maxLength) =>
        new($"compendium.fundamentals.{field}.too-long", $"The {field} must have at most {maxLength} characters.");

    public static DomainError InvalidCode(string field) =>
        new($"compendium.fundamentals.{field}.invalid", $"The {field} must contain only uppercase letters, numbers, dot, underscore or hyphen.");

    public static DomainError InvalidStatus(string field) =>
        new($"compendium.fundamentals.{field}.invalid", $"The {field} is not supported.");

    public static DomainError InvalidHitDie(int die) =>
        new("compendium.fundamentals.hit-die.invalid", $"The hit die d{die} is not supported.");
}
