using Compendium.Application.Errors;
using Compendium.Domain.SharedKernel;
using Compendium.Domain.Sources;

namespace Compendium.Application.Sources;

public sealed class CreateRulesetUseCase
{
    private readonly IRulesetRepository repository;
    private readonly IClock clock;

    public CreateRulesetUseCase(IRulesetRepository repository, IClock clock)
    {
        this.repository = repository;
        this.clock = clock;
    }

    public async Task<ApplicationResult<RulesetDto>> ExecuteAsync(CreateRulesetCommand command, CancellationToken cancellationToken)
    {
        var code = RulesetCode.Create(command.Code);
        var name = RulesetName.Create(command.Name);
        var version = RulesetVersion.Create(command.Version);

        if (code.IsFailure) return ApplicationResult<RulesetDto>.Failure(SourceErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<RulesetDto>.Failure(SourceErrors.FromDomain(name.Error));
        if (version.IsFailure) return ApplicationResult<RulesetDto>.Failure(SourceErrors.FromDomain(version.Error));

        if (await repository.ExistsByCodeAsync(code.Value, cancellationToken))
        {
            return ApplicationResult<RulesetDto>.Failure(SourceErrors.RulesetCodeAlreadyExists(code.Value.Value));
        }

        var ruleset = Ruleset.Create(code.Value, name.Value, version.Value, command.Status, clock.UtcNow);
        if (ruleset.IsFailure)
        {
            return ApplicationResult<RulesetDto>.Failure(SourceErrors.FromDomain(ruleset.Error));
        }

        await repository.AddAsync(ruleset.Value, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return ApplicationResult<RulesetDto>.Success(ruleset.Value.ToDto());
    }
}

public sealed class UpdateRulesetUseCase
{
    private readonly IRulesetRepository repository;
    private readonly IClock clock;

    public UpdateRulesetUseCase(IRulesetRepository repository, IClock clock)
    {
        this.repository = repository;
        this.clock = clock;
    }

    public async Task<ApplicationResult<RulesetDto>> ExecuteAsync(UpdateRulesetCommand command, CancellationToken cancellationToken)
    {
        var code = RulesetCode.Create(command.Code);
        var name = RulesetName.Create(command.Name);
        var version = RulesetVersion.Create(command.Version);

        if (code.IsFailure) return ApplicationResult<RulesetDto>.Failure(SourceErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<RulesetDto>.Failure(SourceErrors.FromDomain(name.Error));
        if (version.IsFailure) return ApplicationResult<RulesetDto>.Failure(SourceErrors.FromDomain(version.Error));

        var ruleset = await repository.GetByCodeAsync(code.Value, cancellationToken);
        if (ruleset is null)
        {
            return ApplicationResult<RulesetDto>.Failure(SourceErrors.RulesetNotFound(code.Value.Value));
        }

        var update = ruleset.Update(name.Value, version.Value, command.Status, clock.UtcNow);
        if (update.IsFailure)
        {
            return ApplicationResult<RulesetDto>.Failure(SourceErrors.FromDomain(update.Error));
        }

        await repository.SaveChangesAsync(cancellationToken);
        return ApplicationResult<RulesetDto>.Success(ruleset.ToDto());
    }
}

public sealed class GetRulesetByCodeQuery
{
    private readonly IRulesetRepository repository;

    public GetRulesetByCodeQuery(IRulesetRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<RulesetDto>> ExecuteAsync(string codeValue, CancellationToken cancellationToken)
    {
        var code = RulesetCode.Create(codeValue);
        if (code.IsFailure)
        {
            return ApplicationResult<RulesetDto>.Failure(SourceErrors.FromDomain(code.Error));
        }

        var ruleset = await repository.GetByCodeAsync(code.Value, cancellationToken);
        return ruleset is null
            ? ApplicationResult<RulesetDto>.Failure(SourceErrors.RulesetNotFound(code.Value.Value))
            : ApplicationResult<RulesetDto>.Success(ruleset.ToDto());
    }
}

internal static class RulesetMapping
{
    public static RulesetDto ToDto(this Ruleset ruleset) =>
        new(ruleset.Id.Value, ruleset.Code.Value, ruleset.Name.Value, ruleset.Version.Value, ruleset.Status);
}
