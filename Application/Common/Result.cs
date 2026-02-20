



using Domain.Common;

namespace Application.Common;

public record Result
{
    public bool isSuccess { get; set; }
    public Error? Error { get; set; }
    
    public Result(bool isSuccess, Error? error)
    {
        isSuccess = isSuccess;
        Error = error;
    }
    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error ?? throw new ArgumentNullException(nameof(error)));

    public static implicit operator Result(Error error) => Failure(error);
}

public record Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, null) => Value = value;
    private Result(Error error) : base(false, error) { }

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);
}