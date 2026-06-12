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

    public static DomainError DuplicateAbilityScoreMethodRule(string code) =>
        new("compendium.fundamentals.ability-score-method-rule.duplicate", $"The ability score method rule '{code}' is duplicated.");

    public static DomainError StandardArrayRequiresSixValues() =>
        new("compendium.fundamentals.ability-score-method.standard-array-values.required", "Standard Array requires exactly six ordered values.");

    public static DomainError InvalidAbilityScore(int score) =>
        new("compendium.fundamentals.ability-score.invalid", $"Ability score '{score}' is invalid.");

    public static DomainError PointBuyRequiresCosts() =>
        new("compendium.fundamentals.ability-score-method.point-buy-costs.required", "Point Buy requires costs by score.");

    public static DomainError InvalidPointBuyCost(int score) =>
        new("compendium.fundamentals.ability-score-method.point-buy-cost.invalid", $"Point Buy cost for score '{score}' is invalid.");

    public static DomainError DuplicatePointBuyScore(int score) =>
        new("compendium.fundamentals.ability-score-method.point-buy-score.duplicate", $"Point Buy score '{score}' is duplicated.");

    public static DomainError RollRuleRequired() =>
        new("compendium.fundamentals.ability-score-method.roll-rule.required", "Random roll requires a roll rule.");

    public static DomainError InvalidRollDice() =>
        new("compendium.fundamentals.ability-score-method.roll-dice.invalid", "Random roll dice quantity and die size must be positive.");

    public static DomainError InvalidRollKeepRule() =>
        new("compendium.fundamentals.ability-score-method.roll-keep.invalid", "Random roll keep rule must keep at least one die and no more than the dice quantity.");

    public static DomainError InvalidRollDropRule() =>
        new("compendium.fundamentals.ability-score-method.roll-drop.invalid", "Random roll drop rule must drop at least one die and fewer dice than rolled.");
}
