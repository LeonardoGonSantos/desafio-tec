namespace FluxoCaixa.Shared.Results;

public static class ResultExtensions
{
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string error)
    {
        if (result.IsFailure)
            return result;

        return predicate(result.Value!) ? result : Result<T>.Failure(error);
    }

    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess && result.Value != null)
            action(result.Value);

        return result;
    }
}
