namespace Compendium.Domain.SharedKernel;

public sealed record CompendiumEntityId : EntityId
{
    private CompendiumEntityId(Guid value)
        : base(value)
    {
    }

    public static CompendiumEntityId New() => new(Guid.CreateVersion7());

    public static Result<CompendiumEntityId> Create(Guid value)
    {
        return value == Guid.Empty
            ? Result<CompendiumEntityId>.Failure(DomainErrors.InvalidEntityId())
            : Result<CompendiumEntityId>.Success(new CompendiumEntityId(value));
    }

    public static Result<CompendiumEntityId> Parse(string? value)
    {
        return Guid.TryParse(value, out var parsed)
            ? Create(parsed)
            : Result<CompendiumEntityId>.Failure(DomainErrors.InvalidEntityId());
    }
}
