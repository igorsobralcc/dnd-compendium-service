using Compendium.Domain.Fundamentals;
using Compendium.Domain.Importing;

namespace Compendium.Application.Importing;

public sealed record SeedNamedEntry(string Code, string Name);
public sealed record SeedSkillEntry(string Code, string Name, string DefaultAbilityCode);
public sealed record SeedProficiencyEntry(string Code, string Name, ProficiencyType Type);
public sealed record SeedEquipmentEntry(string Code, string Name, string Category, decimal Weight, decimal CostAmount, string Currency, string? Description);

public sealed record ImportSourceVersionCommand(
    Guid SourceVersionId,
    string CorrelationId,
    IReadOnlyCollection<SeedNamedEntry> Abilities,
    IReadOnlyCollection<SeedSkillEntry> Skills,
    IReadOnlyCollection<SeedNamedEntry> Languages,
    IReadOnlyCollection<SeedProficiencyEntry> Proficiencies,
    IReadOnlyCollection<int> HitDice,
    IReadOnlyCollection<SeedEquipmentEntry> Equipment);

public sealed record ImportSourceVersionResult(Guid ImportId, Guid SourceVersionId, bool AlreadyImported, int ImportedEntityCount);

public sealed record ValidationIssueDto(Guid Id, string Code, ValidationIssueSeverity Severity, string Message);
public sealed record ValidateSourceVersionResult(Guid SourceVersionId, bool CanPublish, IReadOnlyCollection<ValidationIssueDto> Issues);

public interface ISourceVersionImportGateway
{
    Task<ImportSourceVersionResult> ImportAsync(ImportSourceVersionCommand command, CancellationToken cancellationToken);
}

public interface ISourceVersionValidationGateway
{
    Task<SourceVersionContentSummary?> GetSummaryAsync(Guid sourceVersionId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ValidationIssueDto>> ReplaceIssuesAsync(Guid sourceVersionId, IReadOnlyCollection<ConsistencyIssue> issues, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ValidationIssueDto>> ListIssuesAsync(Guid sourceVersionId, CancellationToken cancellationToken);
}
