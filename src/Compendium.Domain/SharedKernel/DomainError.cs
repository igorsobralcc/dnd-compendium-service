namespace Compendium.Domain.SharedKernel;

public sealed record DomainError(
    string Code,
    string Message,
    DomainErrorKind Kind = DomainErrorKind.Validation)
{
    public static readonly DomainError None = new("domain.none", string.Empty, DomainErrorKind.None);
}

public enum DomainErrorKind
{
    None = 0,
    Validation = 1,
    Conflict = 2,
    NotFound = 3,
    Unauthorized = 4,
    Forbidden = 5,
    Unexpected = 6
}
