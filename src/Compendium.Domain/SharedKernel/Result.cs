namespace Compendium.Domain.SharedKernel;

public sealed class Result
{
    private Result(bool isSuccess, DomainError error)
    {
        if (isSuccess && error != DomainError.None)
        {
            throw new InvalidOperationException("Successful results cannot carry an error.");
        }

        if (!isSuccess && error == DomainError.None)
        {
            throw new InvalidOperationException("Failed results must carry an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public DomainError Error { get; }

    public static Result Success() => new(true, DomainError.None);

    public static Result Failure(DomainError error) => new(false, error);
}

public sealed class Result<T>
{
    private readonly T? value;

    private Result(T value)
    {
        this.value = value;
        IsSuccess = true;
        Error = DomainError.None;
    }

    private Result(DomainError error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public DomainError Error { get; }

    public T Value => IsSuccess
        ? value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(DomainError error) => new(error);
}
