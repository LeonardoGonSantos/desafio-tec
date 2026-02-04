namespace FluxoCaixa.Shared.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public List<string> Errors { get; }

    protected Result(bool isSuccess, List<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? new List<string>();
    }

    public static Result Success() => new(true, new List<string>());

    public static Result Failure(string error) => new(false, new List<string> { error });

    public static Result Failure(List<string> errors) => new(false, errors);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, List<string> errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, new List<string>());

    public static new Result<T> Failure(string error) => new(false, default, new List<string> { error });

    public static new Result<T> Failure(List<string> errors) => new(false, default, errors);
}
