using Compendium.Application.Errors;
using Compendium.Application.Sources;
using Compendium.Domain.Fundamentals;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Fundamentals;

public sealed class CreateAbilityScoreMethodUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IAbilityScoreMethodRepository methods;
    private readonly IClock clock;

    public CreateAbilityScoreMethodUseCase(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        IAbilityScoreMethodRepository methods,
        IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.methods = methods;
        this.clock = clock;
    }

    public async Task<ApplicationResult<AbilityScoreMethodDto>> ExecuteAsync(
        CreateAbilityScoreMethodCommand command,
        CancellationToken cancellationToken)
    {
        var ruleSourceId = CompendiumEntityId.Create(command.RuleSourceId);
        var sourceVersionId = CompendiumEntityId.Create(command.SourceVersionId);
        var code = AbilityScoreMethodCode.Create(command.Code);
        var name = DisplayName.Create(command.Name);
        var ruleInputs = CreateRuleInputs(command.Rules ?? []);

        if (ruleSourceId.IsFailure) return ApplicationResult<AbilityScoreMethodDto>.Failure(FundamentalErrors.FromDomain(ruleSourceId.Error));
        if (sourceVersionId.IsFailure) return ApplicationResult<AbilityScoreMethodDto>.Failure(FundamentalErrors.FromDomain(sourceVersionId.Error));
        if (code.IsFailure) return ApplicationResult<AbilityScoreMethodDto>.Failure(FundamentalErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<AbilityScoreMethodDto>.Failure(FundamentalErrors.FromDomain(name.Error));
        if (ruleInputs.IsFailure) return ApplicationResult<AbilityScoreMethodDto>.Failure(FundamentalErrors.FromDomain(ruleInputs.Error));

        var method = AbilityScoreMethod.Create(
            ruleSourceId.Value,
            sourceVersionId.Value,
            code.Value,
            name.Value,
            command.Type,
            ruleInputs.Value,
            command.StandardValues ?? [],
            (command.PointBuyCosts ?? [])
                .Select(cost => new AbilityScorePointBuyCostInput(cost.Score, cost.Cost))
                .ToArray(),
            command.RollRule is null
                ? null
                : new AbilityScoreRollRuleInput(
                    command.RollRule.DiceQuantity,
                    command.RollRule.DieSize,
                    command.RollRule.KeepHighestQuantity,
                    command.RollRule.DropLowestQuantity,
                    command.RollRule.Repetitions),
            clock.UtcNow);

        if (method.IsFailure)
        {
            return ApplicationResult<AbilityScoreMethodDto>.Failure(FundamentalErrors.FromDomain(method.Error));
        }

        var source = await FundamentalSourceReference.ValidateAsync(
            sources,
            versions,
            command.RuleSourceId,
            command.SourceVersionId,
            cancellationToken);
        if (source.IsFailure) return ApplicationResult<AbilityScoreMethodDto>.Failure(source.Error);

        if (await methods.ExistsByCodeAsync(code.Value, cancellationToken))
        {
            return ApplicationResult<AbilityScoreMethodDto>.Failure(FundamentalErrors.AbilityScoreMethodCodeAlreadyExists(code.Value.Value));
        }

        await methods.AddAsync(method.Value, cancellationToken);
        await methods.SaveChangesAsync(cancellationToken);
        return ApplicationResult<AbilityScoreMethodDto>.Success(method.Value.ToDto());
    }

    private static Result<IReadOnlyCollection<AbilityScoreMethodRuleInput>> CreateRuleInputs(
        IReadOnlyCollection<CreateAbilityScoreMethodRuleCommand> commands)
    {
        var inputs = new List<AbilityScoreMethodRuleInput>();
        foreach (var command in commands)
        {
            var code = AbilityScoreMethodRuleCode.Create(command.Code);
            if (code.IsFailure)
            {
                return Result<IReadOnlyCollection<AbilityScoreMethodRuleInput>>.Failure(code.Error);
            }

            inputs.Add(new AbilityScoreMethodRuleInput(code.Value, command.NumericValue, command.TextValue));
        }

        return Result<IReadOnlyCollection<AbilityScoreMethodRuleInput>>.Success(inputs);
    }
}

public sealed class ListAbilityScoreMethodsQuery
{
    private readonly IAbilityScoreMethodRepository repository;

    public ListAbilityScoreMethodsQuery(IAbilityScoreMethodRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<IReadOnlyCollection<AbilityScoreMethodDto>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var methods = await repository.ListAsync(cancellationToken);
        return ApplicationResult<IReadOnlyCollection<AbilityScoreMethodDto>>.Success(methods.Select(method => method.ToDto()).ToArray());
    }
}

internal static class AbilityScoreMethodMapping
{
    public static AbilityScoreMethodDto ToDto(this AbilityScoreMethod method) =>
        new(
            method.Id.Value,
            method.RuleSourceId.Value,
            method.SourceVersionId.Value,
            method.Code.Value,
            method.Name.Value,
            method.Type,
            method.Rules
                .OrderBy(rule => rule.Code.Value)
                .Select(rule => new AbilityScoreMethodRuleDto(rule.Id.Value, rule.Code.Value, rule.NumericValue, rule.TextValue))
                .ToArray(),
            method.StandardValues
                .OrderBy(value => value.Position)
                .Select(value => new AbilityScoreStandardValueDto(value.Id.Value, value.Position, value.Score))
                .ToArray(),
            method.PointBuyCosts
                .OrderBy(cost => cost.Score)
                .Select(cost => new AbilityScorePointBuyCostDto(cost.Id.Value, cost.Score, cost.Cost))
                .ToArray(),
            method.RollRules
                .Select(rule => new AbilityScoreRollRuleDto(
                    rule.Id.Value,
                    rule.DiceQuantity,
                    rule.DieSize,
                    rule.KeepHighestQuantity,
                    rule.DropLowestQuantity,
                    rule.Repetitions))
                .SingleOrDefault());
}
