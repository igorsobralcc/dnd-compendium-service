using Compendium.Application.Errors;
using Compendium.Domain.SharedKernel;
using Compendium.Domain.Sources;

namespace Compendium.Application.Sources;

public sealed class CreateRuleSourceUseCase
{
    private readonly IRulesetRepository rulesets;
    private readonly IRuleSourceRepository sources;
    private readonly IClock clock;

    public CreateRuleSourceUseCase(IRulesetRepository rulesets, IRuleSourceRepository sources, IClock clock)
    {
        this.rulesets = rulesets;
        this.sources = sources;
        this.clock = clock;
    }

    public async Task<ApplicationResult<RuleSourceDto>> ExecuteAsync(CreateRuleSourceCommand command, CancellationToken cancellationToken)
    {
        var rulesetId = CompendiumEntityId.Create(command.RulesetId);
        var code = SourceCode.Create(command.Code);
        var name = SourceName.Create(command.Name);

        if (rulesetId.IsFailure) return ApplicationResult<RuleSourceDto>.Failure(SourceErrors.FromDomain(rulesetId.Error));
        if (code.IsFailure) return ApplicationResult<RuleSourceDto>.Failure(SourceErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<RuleSourceDto>.Failure(SourceErrors.FromDomain(name.Error));

        if (await rulesets.GetByIdAsync(rulesetId.Value, cancellationToken) is null)
        {
            return ApplicationResult<RuleSourceDto>.Failure(SourceErrors.RulesetNotFound(rulesetId.Value.ToString()));
        }

        if (await sources.ExistsByRulesetAndCodeAsync(rulesetId.Value, code.Value, cancellationToken))
        {
            return ApplicationResult<RuleSourceDto>.Failure(SourceErrors.RuleSourceCodeAlreadyExists(code.Value.Value));
        }

        var source = RuleSource.Create(rulesetId.Value, code.Value, name.Value, command.Type, command.Status, clock.UtcNow);
        if (source.IsFailure)
        {
            return ApplicationResult<RuleSourceDto>.Failure(SourceErrors.FromDomain(source.Error));
        }

        await sources.AddAsync(source.Value, cancellationToken);
        await sources.SaveChangesAsync(cancellationToken);
        return ApplicationResult<RuleSourceDto>.Success(source.Value.ToDto());
    }
}

public sealed class ActivateRuleSourceUseCase
{
    private readonly IRuleSourceRepository repository;
    private readonly IClock clock;

    public ActivateRuleSourceUseCase(IRuleSourceRepository repository, IClock clock)
    {
        this.repository = repository;
        this.clock = clock;
    }

    public async Task<ApplicationResult<RuleSourceDto>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var source = await GetSourceAsync(id, cancellationToken);
        if (source.IsFailure) return ApplicationResult<RuleSourceDto>.Failure(source.Error);

        source.Value.Activate(clock.UtcNow);
        await repository.SaveChangesAsync(cancellationToken);
        return ApplicationResult<RuleSourceDto>.Success(source.Value.ToDto());
    }

    private async Task<ApplicationResult<RuleSource>> GetSourceAsync(Guid id, CancellationToken cancellationToken)
    {
        var sourceId = CompendiumEntityId.Create(id);
        if (sourceId.IsFailure) return ApplicationResult<RuleSource>.Failure(SourceErrors.FromDomain(sourceId.Error));

        var source = await repository.GetByIdAsync(sourceId.Value, cancellationToken);
        return source is null
            ? ApplicationResult<RuleSource>.Failure(SourceErrors.RuleSourceNotFound(sourceId.Value.ToString()))
            : ApplicationResult<RuleSource>.Success(source);
    }
}

public sealed class DeactivateRuleSourceUseCase
{
    private readonly IRuleSourceRepository repository;
    private readonly IClock clock;

    public DeactivateRuleSourceUseCase(IRuleSourceRepository repository, IClock clock)
    {
        this.repository = repository;
        this.clock = clock;
    }

    public async Task<ApplicationResult<RuleSourceDto>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var sourceId = CompendiumEntityId.Create(id);
        if (sourceId.IsFailure) return ApplicationResult<RuleSourceDto>.Failure(SourceErrors.FromDomain(sourceId.Error));

        var source = await repository.GetByIdAsync(sourceId.Value, cancellationToken);
        if (source is null)
        {
            return ApplicationResult<RuleSourceDto>.Failure(SourceErrors.RuleSourceNotFound(sourceId.Value.ToString()));
        }

        source.Deactivate(clock.UtcNow);
        await repository.SaveChangesAsync(cancellationToken);
        return ApplicationResult<RuleSourceDto>.Success(source.ToDto());
    }
}

public sealed class ListRuleSourcesByRulesetQuery
{
    private readonly IRuleSourceRepository repository;

    public ListRuleSourcesByRulesetQuery(IRuleSourceRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<IReadOnlyCollection<RuleSourceDto>>> ExecuteAsync(
        Guid rulesetIdValue,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var rulesetId = CompendiumEntityId.Create(rulesetIdValue);
        if (rulesetId.IsFailure)
        {
            return ApplicationResult<IReadOnlyCollection<RuleSourceDto>>.Failure(SourceErrors.FromDomain(rulesetId.Error));
        }

        var sources = await repository.ListByRulesetAsync(rulesetId.Value, includeInactive, cancellationToken);
        return ApplicationResult<IReadOnlyCollection<RuleSourceDto>>.Success(sources.Select(s => s.ToDto()).ToArray());
    }
}

internal static class RuleSourceMapping
{
    public static RuleSourceDto ToDto(this RuleSource source) =>
        new(source.Id.Value, source.RulesetId.Value, source.Code.Value, source.Name.Value, source.Type, source.Status);
}
