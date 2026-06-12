namespace Compendium.Application.Errors;

public sealed class ApplicationResult
{
    private ApplicationResult(bool isSuccess, ApplicationError error)
    {
        if (isSuccess && error != ApplicationError.None)
        {
            throw new InvalidOperationException("Successful results cannot carry an error.");
        }

        if (!isSuccess && error == ApplicationError.None)
        {
            throw new InvalidOperationException("Failed results must carry an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ApplicationError Error { get; }

    public static ApplicationResult Success() => new(true, ApplicationError.None);

    public static ApplicationResult Failure(ApplicationError error) => new(false, error);
}

public sealed class ApplicationResult<T>
{
    private readonly T? value;

    private ApplicationResult(T value)
    {
        this.value = value;
        IsSuccess = true;
        Error = ApplicationError.None;
    }

    private ApplicationResult(ApplicationError error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ApplicationError Error { get; }

    public T Value => IsSuccess
        ? value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    public static ApplicationResult<T> Success(T value) => new(value);

    public static ApplicationResult<T> Failure(ApplicationError error) => new(error);
}
