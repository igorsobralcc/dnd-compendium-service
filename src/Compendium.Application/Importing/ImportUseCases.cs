using Compendium.Application.Errors;
using Compendium.Domain.Importing;

namespace Compendium.Application.Importing;

public sealed class ImportSourceVersionUseCase
{
    private readonly ISourceVersionImportGateway gateway;
    public ImportSourceVersionUseCase(ISourceVersionImportGateway gateway) => this.gateway = gateway;

    public async Task<ApplicationResult<ImportSourceVersionResult>> ExecuteAsync(ImportSourceVersionCommand command, CancellationToken cancellationToken)
    {
        if (command.SourceVersionId == Guid.Empty)
            return ApplicationResult<ImportSourceVersionResult>.Failure(new ApplicationError("import.source-version-required", "Source version is required.", ApplicationErrorKind.Validation));
        if (string.IsNullOrWhiteSpace(command.CorrelationId))
            return ApplicationResult<ImportSourceVersionResult>.Failure(new ApplicationError("import.correlation-required", "Correlation id is required.", ApplicationErrorKind.Validation));

        try
        {
            return ApplicationResult<ImportSourceVersionResult>.Success(await gateway.ImportAsync(command, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return ApplicationResult<ImportSourceVersionResult>.Failure(new ApplicationError("import.invalid", exception.Message, ApplicationErrorKind.Validation));
        }
    }
}

public sealed class ValidateSourceVersionUseCase
{
    private readonly ISourceVersionValidationGateway gateway;
    private readonly CompendiumConsistencyChecker checker;

    public ValidateSourceVersionUseCase(ISourceVersionValidationGateway gateway, CompendiumConsistencyChecker checker)
    {
        this.gateway = gateway;
        this.checker = checker;
    }

    public async Task<ApplicationResult<ValidateSourceVersionResult>> ExecuteAsync(Guid sourceVersionId, CancellationToken cancellationToken)
    {
        var summary = await gateway.GetSummaryAsync(sourceVersionId, cancellationToken);
        if (summary is null)
            return ApplicationResult<ValidateSourceVersionResult>.Failure(new ApplicationError("import.source-version-not-found", "Source version was not found.", ApplicationErrorKind.NotFound));

        var issues = await gateway.ReplaceIssuesAsync(sourceVersionId, checker.Check(summary), cancellationToken);
        return ApplicationResult<ValidateSourceVersionResult>.Success(new(sourceVersionId, issues.All(x => x.Severity != ValidationIssueSeverity.Blocker), issues));
    }
}

public sealed class ListSourceVersionValidationIssuesQuery
{
    private readonly ISourceVersionValidationGateway gateway;
    public ListSourceVersionValidationIssuesQuery(ISourceVersionValidationGateway gateway) => this.gateway = gateway;

    public async Task<ApplicationResult<IReadOnlyCollection<ValidationIssueDto>>> ExecuteAsync(Guid sourceVersionId, CancellationToken cancellationToken) =>
        ApplicationResult<IReadOnlyCollection<ValidationIssueDto>>.Success(await gateway.ListIssuesAsync(sourceVersionId, cancellationToken));
}
