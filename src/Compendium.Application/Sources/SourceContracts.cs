using Compendium.Domain.Sources;

namespace Compendium.Application.Sources;

public sealed record CreateRulesetCommand(string Code, string Name, string Version, RulesetStatus Status);

public sealed record UpdateRulesetCommand(string Code, string Name, string Version, RulesetStatus Status);

public sealed record RulesetDto(Guid Id, string Code, string Name, string Version, RulesetStatus Status);

public sealed record CreateRuleSourceCommand(Guid RulesetId, string Code, string Name, SourceType Type, SourceStatus Status);

public sealed record RuleSourceDto(Guid Id, Guid RulesetId, string Code, string Name, SourceType Type, SourceStatus Status);

public sealed record CreateSourceVersionCommand(
    Guid RuleSourceId,
    string VersionNumber,
    DateOnly PublicationDate,
    ImportStatus ImportStatus,
    bool IsCurrent);

public sealed record SourceVersionDto(
    Guid Id,
    Guid RuleSourceId,
    string VersionNumber,
    DateOnly PublicationDate,
    ImportStatus ImportStatus,
    bool IsCurrent);
