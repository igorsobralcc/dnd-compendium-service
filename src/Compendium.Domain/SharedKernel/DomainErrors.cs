namespace Compendium.Domain.SharedKernel;

public static class DomainErrors
{
    public static DomainError InvalidEntityId(string valueName = "id") =>
        new(
            "compendium.entity-id.invalid",
            $"The {valueName} must be a non-empty GUID.",
            DomainErrorKind.Validation);

    public static DomainError InvalidPagination(string reason) =>
        new("compendium.pagination.invalid", reason, DomainErrorKind.Validation);
}
