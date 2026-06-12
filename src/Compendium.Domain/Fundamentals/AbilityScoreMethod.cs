using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Fundamentals;

public sealed class AbilityScoreMethod
{
    private readonly List<AbilityScoreMethodRule> rules = [];
    private readonly List<AbilityScoreStandardValue> standardValues = [];
    private readonly List<AbilityScorePointBuyCost> pointBuyCosts = [];
    private readonly List<AbilityScoreRollRule> rollRules = [];

    private AbilityScoreMethod()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private AbilityScoreMethod(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        AbilityScoreMethodCode code,
        DisplayName name,
        AbilityScoreMethodType type)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
        Type = type;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId RuleSourceId { get; private set; }

    public CompendiumEntityId SourceVersionId { get; private set; }

    public AbilityScoreMethodCode Code { get; private set; }

    public DisplayName Name { get; private set; }

    public AbilityScoreMethodType Type { get; private set; }

    public IReadOnlyCollection<AbilityScoreMethodRule> Rules => rules;

    public IReadOnlyCollection<AbilityScoreStandardValue> StandardValues => standardValues;

    public IReadOnlyCollection<AbilityScorePointBuyCost> PointBuyCosts => pointBuyCosts;

    public IReadOnlyCollection<AbilityScoreRollRule> RollRules => rollRules;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<AbilityScoreMethod> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        AbilityScoreMethodCode code,
        DisplayName name,
        AbilityScoreMethodType type,
        IReadOnlyCollection<AbilityScoreMethodRuleInput> ruleInputs,
        IReadOnlyCollection<int> standardValueInputs,
        IReadOnlyCollection<AbilityScorePointBuyCostInput> pointBuyCostInputs,
        AbilityScoreRollRuleInput? rollRuleInput,
        DateTimeOffset now)
    {
        if (!Enum.IsDefined(type))
        {
            return Result<AbilityScoreMethod>.Failure(FundamentalDomainErrors.InvalidStatus("ability-score-method-type"));
        }

        var method = new AbilityScoreMethod(CompendiumEntityId.New(), ruleSourceId, sourceVersionId, code, name, type)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        var rulesResult = method.AddRules(ruleInputs);
        if (rulesResult.IsFailure) return Result<AbilityScoreMethod>.Failure(rulesResult.Error);

        var typeResult = type switch
        {
            AbilityScoreMethodType.StandardArray => method.ConfigureStandardArray(standardValueInputs),
            AbilityScoreMethodType.PointBuy => method.ConfigurePointBuy(pointBuyCostInputs),
            AbilityScoreMethodType.RandomRoll => method.ConfigureRandomRoll(rollRuleInput),
            _ => Result.Success()
        };

        return typeResult.IsFailure
            ? Result<AbilityScoreMethod>.Failure(typeResult.Error)
            : Result<AbilityScoreMethod>.Success(method);
    }

    private Result AddRules(IReadOnlyCollection<AbilityScoreMethodRuleInput> ruleInputs)
    {
        var seenCodes = new HashSet<string>(StringComparer.Ordinal);
        foreach (var input in ruleInputs)
        {
            if (!seenCodes.Add(input.Code.Value))
            {
                return Result.Failure(FundamentalDomainErrors.DuplicateAbilityScoreMethodRule(input.Code.Value));
            }

            rules.Add(AbilityScoreMethodRule.Create(CompendiumEntityId.New(), Id, input.Code, input.NumericValue, input.TextValue));
        }

        return Result.Success();
    }

    private Result ConfigureStandardArray(IReadOnlyCollection<int> values)
    {
        if (values.Count != 6)
        {
            return Result.Failure(FundamentalDomainErrors.StandardArrayRequiresSixValues());
        }

        var position = 1;
        foreach (var score in values)
        {
            if (score <= 0)
            {
                return Result.Failure(FundamentalDomainErrors.InvalidAbilityScore(score));
            }

            standardValues.Add(AbilityScoreStandardValue.Create(CompendiumEntityId.New(), Id, position++, score));
        }

        return Result.Success();
    }

    private Result ConfigurePointBuy(IReadOnlyCollection<AbilityScorePointBuyCostInput> costs)
    {
        if (costs.Count == 0)
        {
            return Result.Failure(FundamentalDomainErrors.PointBuyRequiresCosts());
        }

        var seenScores = new HashSet<int>();
        foreach (var cost in costs.OrderBy(cost => cost.Score))
        {
            if (cost.Score <= 0)
            {
                return Result.Failure(FundamentalDomainErrors.InvalidAbilityScore(cost.Score));
            }

            if (cost.Cost < 0)
            {
                return Result.Failure(FundamentalDomainErrors.InvalidPointBuyCost(cost.Score));
            }

            if (!seenScores.Add(cost.Score))
            {
                return Result.Failure(FundamentalDomainErrors.DuplicatePointBuyScore(cost.Score));
            }

            pointBuyCosts.Add(AbilityScorePointBuyCost.Create(CompendiumEntityId.New(), Id, cost.Score, cost.Cost));
        }

        return Result.Success();
    }

    private Result ConfigureRandomRoll(AbilityScoreRollRuleInput? rollRule)
    {
        if (rollRule is null)
        {
            return Result.Failure(FundamentalDomainErrors.RollRuleRequired());
        }

        if (rollRule.DiceQuantity <= 0 || rollRule.DieSize <= 0 || rollRule.Repetitions <= 0)
        {
            return Result.Failure(FundamentalDomainErrors.InvalidRollDice());
        }

        if (rollRule.KeepHighestQuantity <= 0 || rollRule.KeepHighestQuantity > rollRule.DiceQuantity)
        {
            return Result.Failure(FundamentalDomainErrors.InvalidRollKeepRule());
        }

        if (rollRule.DropLowestQuantity.HasValue &&
            (rollRule.DropLowestQuantity.Value <= 0 || rollRule.DropLowestQuantity.Value >= rollRule.DiceQuantity))
        {
            return Result.Failure(FundamentalDomainErrors.InvalidRollDropRule());
        }

        rollRules.Add(AbilityScoreRollRule.Create(
            CompendiumEntityId.New(),
            Id,
            rollRule.DiceQuantity,
            rollRule.DieSize,
            rollRule.KeepHighestQuantity,
            rollRule.DropLowestQuantity,
            rollRule.Repetitions));

        return Result.Success();
    }
}

public sealed class AbilityScoreMethodRule
{
    private AbilityScoreMethodRule()
    {
        AbilityScoreMethodId = null!;
        Code = null!;
    }

    private AbilityScoreMethodRule(
        CompendiumEntityId id,
        CompendiumEntityId abilityScoreMethodId,
        AbilityScoreMethodRuleCode code,
        int? numericValue,
        string? textValue)
    {
        Id = id;
        AbilityScoreMethodId = abilityScoreMethodId;
        Code = code;
        NumericValue = numericValue;
        TextValue = string.IsNullOrWhiteSpace(textValue) ? null : textValue.Trim();
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId AbilityScoreMethodId { get; private set; }

    public AbilityScoreMethodRuleCode Code { get; private set; }

    public int? NumericValue { get; private set; }

    public string? TextValue { get; private set; }

    public static AbilityScoreMethodRule Create(
        CompendiumEntityId id,
        CompendiumEntityId abilityScoreMethodId,
        AbilityScoreMethodRuleCode code,
        int? numericValue,
        string? textValue) =>
        new(id, abilityScoreMethodId, code, numericValue, textValue);
}

public sealed class AbilityScoreStandardValue
{
    private AbilityScoreStandardValue()
    {
        AbilityScoreMethodId = null!;
    }

    private AbilityScoreStandardValue(CompendiumEntityId id, CompendiumEntityId abilityScoreMethodId, int position, int score)
    {
        Id = id;
        AbilityScoreMethodId = abilityScoreMethodId;
        Position = position;
        Score = score;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId AbilityScoreMethodId { get; private set; }

    public int Position { get; private set; }

    public int Score { get; private set; }

    public static AbilityScoreStandardValue Create(
        CompendiumEntityId id,
        CompendiumEntityId abilityScoreMethodId,
        int position,
        int score) =>
        new(id, abilityScoreMethodId, position, score);
}

public sealed class AbilityScorePointBuyCost
{
    private AbilityScorePointBuyCost()
    {
        AbilityScoreMethodId = null!;
    }

    private AbilityScorePointBuyCost(CompendiumEntityId id, CompendiumEntityId abilityScoreMethodId, int score, int cost)
    {
        Id = id;
        AbilityScoreMethodId = abilityScoreMethodId;
        Score = score;
        Cost = cost;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId AbilityScoreMethodId { get; private set; }

    public int Score { get; private set; }

    public int Cost { get; private set; }

    public static AbilityScorePointBuyCost Create(
        CompendiumEntityId id,
        CompendiumEntityId abilityScoreMethodId,
        int score,
        int cost) =>
        new(id, abilityScoreMethodId, score, cost);
}

public sealed class AbilityScoreRollRule
{
    private AbilityScoreRollRule()
    {
        AbilityScoreMethodId = null!;
    }

    private AbilityScoreRollRule(
        CompendiumEntityId id,
        CompendiumEntityId abilityScoreMethodId,
        int diceQuantity,
        int dieSize,
        int keepHighestQuantity,
        int? dropLowestQuantity,
        int repetitions)
    {
        Id = id;
        AbilityScoreMethodId = abilityScoreMethodId;
        DiceQuantity = diceQuantity;
        DieSize = dieSize;
        KeepHighestQuantity = keepHighestQuantity;
        DropLowestQuantity = dropLowestQuantity;
        Repetitions = repetitions;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId AbilityScoreMethodId { get; private set; }

    public int DiceQuantity { get; private set; }

    public int DieSize { get; private set; }

    public int KeepHighestQuantity { get; private set; }

    public int? DropLowestQuantity { get; private set; }

    public int Repetitions { get; private set; }

    public static AbilityScoreRollRule Create(
        CompendiumEntityId id,
        CompendiumEntityId abilityScoreMethodId,
        int diceQuantity,
        int dieSize,
        int keepHighestQuantity,
        int? dropLowestQuantity,
        int repetitions) =>
        new(id, abilityScoreMethodId, diceQuantity, dieSize, keepHighestQuantity, dropLowestQuantity, repetitions);
}

public sealed record AbilityScoreMethodRuleInput(AbilityScoreMethodRuleCode Code, int? NumericValue, string? TextValue);

public sealed record AbilityScorePointBuyCostInput(int Score, int Cost);

public sealed record AbilityScoreRollRuleInput(
    int DiceQuantity,
    int DieSize,
    int KeepHighestQuantity,
    int? DropLowestQuantity,
    int Repetitions);
