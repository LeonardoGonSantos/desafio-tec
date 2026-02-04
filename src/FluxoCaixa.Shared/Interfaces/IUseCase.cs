using FluxoCaixa.Shared.Results;

namespace FluxoCaixa.Shared.Interfaces;

public interface IUseCase<in TRequest, TResponse>
{
    Task<Result<TResponse>> ExecuteAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}
