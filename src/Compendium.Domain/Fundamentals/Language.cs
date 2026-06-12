using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Fundamentals;

public sealed class Language
{
    private Language()
    {
        RuleSourceId = null!;
        SourceVersionId = null!;
        Code = null!;
        Name = null!;
    }

    private Language(
        CompendiumEntityId id,
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        LanguageCode code,
        DisplayName name)
    {
        Id = id;
        RuleSourceId = ruleSourceId;
        SourceVersionId = sourceVersionId;
        Code = code;
        Name = name;
    }

    public CompendiumEntityId Id { get; private set; } = null!;

    public CompendiumEntityId RuleSourceId { get; private set; }

    public CompendiumEntityId SourceVersionId { get; private set; }

    public LanguageCode Code { get; private set; }

    public DisplayName Name { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<Language> Create(
        CompendiumEntityId ruleSourceId,
        CompendiumEntityId sourceVersionId,
        LanguageCode code,
        DisplayName name,
        DateTimeOffset now)
    {
        var language = new Language(CompendiumEntityId.New(), ruleSourceId, sourceVersionId, code, name)
        {
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        return Result<Language>.Success(language);
    }

    public void Update(DisplayName name, CompendiumEntityId sourceVersionId, DateTimeOffset now)
    {
        Name = name;
        SourceVersionId = sourceVersionId;
        UpdatedAtUtc = now;
    }
}
