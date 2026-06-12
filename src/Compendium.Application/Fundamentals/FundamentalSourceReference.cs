using Compendium.Application.Errors;
using Compendium.Application.Sources;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Fundamentals;

internal sealed record SourceReference(CompendiumEntityId RuleSourceId, CompendiumEntityId SourceVersionId);

internal static class FundamentalSourceReference
{
    public static async Task<ApplicationResult<SourceReference>> ValidateAsync(
        IRuleSourceRepository sources,
        ISourceVersionRepository versions,
        Guid ruleSourceIdValue,
        Guid sourceVersionIdValue,
        CancellationToken cancellationToken)
    {
        var ruleSourceId = CompendiumEntityId.Create(ruleSourceIdValue);
        var sourceVersionId = CompendiumEntityId.Create(sourceVersionIdValue);

        if (ruleSourceId.IsFailure)
        {
            return ApplicationResult<SourceReference>.Failure(FundamentalErrors.FromDomain(ruleSourceId.Error));
        }

        if (sourceVersionId.IsFailure)
        {
            return ApplicationResult<SourceReference>.Failure(FundamentalErrors.FromDomain(sourceVersionId.Error));
        }

        if (await sources.GetByIdAsync(ruleSourceId.Value, cancellationToken) is null)
        {
            return ApplicationResult<SourceReference>.Failure(SourceErrors.RuleSourceNotFound(ruleSourceId.Value.ToString()));
        }

        var sourceVersion = await versions.GetByIdAsync(sourceVersionId.Value, cancellationToken);
        if (sourceVersion is null)
        {
            return ApplicationResult<SourceReference>.Failure(FundamentalErrors.SourceVersionNotFound(sourceVersionId.Value.ToString()));
        }

        if (sourceVersion.RuleSourceId != ruleSourceId.Value)
        {
            return ApplicationResult<SourceReference>.Failure(
                FundamentalErrors.SourceVersionDoesNotBelongToSource(sourceVersionId.Value.ToString(), ruleSourceId.Value.ToString()));
        }

        return ApplicationResult<SourceReference>.Success(new SourceReference(ruleSourceId.Value, sourceVersionId.Value));
    }
}
