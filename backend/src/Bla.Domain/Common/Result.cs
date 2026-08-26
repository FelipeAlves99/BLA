namespace Bla.Domain.Common;

public class Result
{
    protected Result(bool isSuccess, string? error) => (IsSuccess, Error) = (isSuccess, error);
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(true, value, null);
    public static Result<T> Failure<T>(string error) => new(false, default!, error);
}

public sealed class Result<T>(bool isSuccess, T value, string? error) : Result(isSuccess, error)
{
    public T Value => IsSuccess ? value : throw new InvalidOperationException("Cannot access a failed result value.");
}
