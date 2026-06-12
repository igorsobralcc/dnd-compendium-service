using Compendium.Application.Errors;
using Compendium.Domain.SharedKernel;
using Compendium.Domain.Sources;

namespace Compendium.Application.Sources;

public sealed class CreateSourceVersionUseCase
{
    private readonly IRuleSourceRepository sources;
    private readonly ISourceVersionRepository versions;
    private readonly IClock clock;

    public CreateSourceVersionUseCase(IRuleSourceRepository sources, ISourceVersionRepository versions, IClock clock)
    {
        this.sources = sources;
        this.versions = versions;
        this.clock = clock;
    }

    public async Task<ApplicationResult<SourceVersionDto>> ExecuteAsync(CreateSourceVersionCommand command, CancellationToken cancellationToken)
    {
        var sourceId = CompendiumEntityId.Create(command.RuleSourceId);
        var versionNumber = SourceVersionNumber.Create(command.VersionNumber);
        var publicationDate = PublicationDate.Create(command.PublicationDate, DateOnly.FromDateTime(clock.UtcNow.UtcDateTime));

        if (sourceId.IsFailure) return ApplicationResult<SourceVersionDto>.Failure(SourceErrors.FromDomain(sourceId.Error));
        if (versionNumber.IsFailure) return ApplicationResult<SourceVersionDto>.Failure(SourceErrors.FromDomain(versionNumber.Error));
        if (publicationDate.IsFailure) return ApplicationResult<SourceVersionDto>.Failure(SourceErrors.FromDomain(publicationDate.Error));

        if (await sources.GetByIdAsync(sourceId.Value, cancellationToken) is null)
        {
            return ApplicationResult<SourceVersionDto>.Failure(SourceErrors.RuleSourceNotFound(sourceId.Value.ToString()));
        }

        if (await versions.ExistsBySourceAndVersionAsync(sourceId.Value, versionNumber.Value, cancellationToken))
        {
            return ApplicationResult<SourceVersionDto>.Failure(SourceErrors.SourceVersionAlreadyExists(versionNumber.Value.Value));
        }

        if (command.IsCurrent)
        {
            var currentVersions = await versions.ListBySourceAsync(sourceId.Value, cancellationToken);
            foreach (var current in currentVersions.Where(v => v.IsCurrent))
            {
                current.MarkAsNotCurrent(clock.UtcNow);
            }
        }

        var sourceVersion = SourceVersion.Create(
            sourceId.Value,
            versionNumber.Value,
            publicationDate.Value,
            command.ImportStatus,
            command.IsCurrent,
            clock.UtcNow);

        if (sourceVersion.IsFailure)
        {
            return ApplicationResult<SourceVersionDto>.Failure(SourceErrors.FromDomain(sourceVersion.Error));
        }

        await versions.AddAsync(sourceVersion.Value, cancellationToken);
        await versions.SaveChangesAsync(cancellationToken);
        return ApplicationResult<SourceVersionDto>.Success(sourceVersion.Value.ToDto());
    }
}

public sealed class MarkSourceVersionAsCurrentUseCase
{
    private readonly ISourceVersionRepository repository;
    private readonly IClock clock;

    public MarkSourceVersionAsCurrentUseCase(ISourceVersionRepository repository, IClock clock)
    {
        this.repository = repository;
        this.clock = clock;
    }

    public async Task<ApplicationResult<SourceVersionDto>> ExecuteAsync(Guid ruleSourceIdValue, Guid versionIdValue, CancellationToken cancellationToken)
    {
        var sourceId = CompendiumEntityId.Create(ruleSourceIdValue);
        var versionId = CompendiumEntityId.Create(versionIdValue);
        if (sourceId.IsFailure) return ApplicationResult<SourceVersionDto>.Failure(SourceErrors.FromDomain(sourceId.Error));
        if (versionId.IsFailure) return ApplicationResult<SourceVersionDto>.Failure(SourceErrors.FromDomain(versionId.Error));

        var versions = await repository.ListBySourceAsync(sourceId.Value, cancellationToken);
        var selected = versions.SingleOrDefault(v => v.Id == versionId.Value);
        if (selected is null)
        {
            return ApplicationResult<SourceVersionDto>.Failure(SourceErrors.CurrentSourceVersionNotFound(sourceId.Value.ToString()));
        }

        foreach (var version in versions)
        {
            if (version.Id == versionId.Value)
            {
                version.MarkAsCurrent(clock.UtcNow);
            }
            else if (version.IsCurrent)
            {
                version.MarkAsNotCurrent(clock.UtcNow);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);
        return ApplicationResult<SourceVersionDto>.Success(selected.ToDto());
    }
}

public sealed class GetCurrentSourceVersionQuery
{
    private readonly ISourceVersionRepository repository;

    public GetCurrentSourceVersionQuery(ISourceVersionRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<SourceVersionDto>> ExecuteAsync(Guid ruleSourceIdValue, CancellationToken cancellationToken)
    {
        var sourceId = CompendiumEntityId.Create(ruleSourceIdValue);
        if (sourceId.IsFailure) return ApplicationResult<SourceVersionDto>.Failure(SourceErrors.FromDomain(sourceId.Error));

        var version = await repository.GetCurrentBySourceAsync(sourceId.Value, cancellationToken);
        return version is null
            ? ApplicationResult<SourceVersionDto>.Failure(SourceErrors.CurrentSourceVersionNotFound(sourceId.Value.ToString()))
            : ApplicationResult<SourceVersionDto>.Success(version.ToDto());
    }
}

public sealed class ListSourceVersionsQuery
{
    private readonly ISourceVersionRepository repository;

    public ListSourceVersionsQuery(ISourceVersionRepository repository) => this.repository = repository;

    public async Task<ApplicationResult<IReadOnlyCollection<SourceVersionDto>>> ExecuteAsync(Guid ruleSourceIdValue, CancellationToken cancellationToken)
    {
        var sourceId = CompendiumEntityId.Create(ruleSourceIdValue);
        if (sourceId.IsFailure)
        {
            return ApplicationResult<IReadOnlyCollection<SourceVersionDto>>.Failure(SourceErrors.FromDomain(sourceId.Error));
        }

        var versions = await repository.ListBySourceAsync(sourceId.Value, cancellationToken);
        return ApplicationResult<IReadOnlyCollection<SourceVersionDto>>.Success(versions.Select(v => v.ToDto()).ToArray());
    }
}

internal static class SourceVersionMapping
{
    public static SourceVersionDto ToDto(this SourceVersion version) =>
        new(
            version.Id.Value,
            version.RuleSourceId.Value,
            version.VersionNumber.Value,
            version.PublicationDate.Value,
            version.ImportStatus,
            version.IsCurrent);
}
