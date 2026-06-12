namespace Compendium.Application.Errors;

public sealed record ApplicationError(
    string Code,
    string Message,
    ApplicationErrorKind Kind = ApplicationErrorKind.Validation)
{
    public static readonly ApplicationError None = new("application.none", string.Empty, ApplicationErrorKind.None);
}

public enum ApplicationErrorKind
{
    None = 0,
    Validation = 1,
    Conflict = 2,
    NotFound = 3,
    Unauthorized = 4,
    Forbidden = 5,
    Unexpected = 6
}
