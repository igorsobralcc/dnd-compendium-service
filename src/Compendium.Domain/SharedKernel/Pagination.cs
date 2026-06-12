namespace Compendium.Domain.SharedKernel;

public sealed record Pagination
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 50;
    public const int MaxPageSize = 200;

    private Pagination(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public int Page { get; }

    public int PageSize { get; }

    public int Offset => (Page - 1) * PageSize;

    public static Result<Pagination> Create(int page = DefaultPage, int pageSize = DefaultPageSize)
    {
        if (page < 1)
        {
            return Result<Pagination>.Failure(DomainErrors.InvalidPagination("Page must be greater than zero."));
        }

        if (pageSize is < 1 or > MaxPageSize)
        {
            return Result<Pagination>.Failure(
                DomainErrors.InvalidPagination($"Page size must be between 1 and {MaxPageSize}."));
        }

        return Result<Pagination>.Success(new Pagination(page, pageSize));
    }
}

public sealed record Page<T>(
    IReadOnlyCollection<T> Items,
    int PageNumber,
    int PageSize,
    long TotalItems);
