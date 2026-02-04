using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Shared.Interfaces;
using FluxoCaixa.Shared.Results;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Application.UseCases.Lancamentos.ObterLancamento;

public interface IObterLancamentoUseCase : IUseCase<ObterLancamentoQuery, ObterLancamentoResponse?>
{
}

public class ObterLancamentoUseCase : IObterLancamentoUseCase
{
    private readonly ILancamentoRepository _repository;
    private readonly ILogger<ObterLancamentoUseCase> _logger;

    public ObterLancamentoUseCase(
        ILancamentoRepository repository,
        ILogger<ObterLancamentoUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ObterLancamentoResponse?>> ExecuteAsync(
        ObterLancamentoQuery query,
        CancellationToken cancellationToken = default)
    {
        var lancamento = await _repository.GetByIdAsync(query.Id, cancellationToken);

        if (lancamento is null)
            return Result<ObterLancamentoResponse?>.Failure("Lançamento não encontrado");

        var response = new ObterLancamentoResponse(
            lancamento.Id,
            lancamento.Tipo.ToString(),
            lancamento.Valor.ValorDecimal,
            lancamento.Descricao,
            lancamento.DataLancamento,
            lancamento.CreatedAt);

        return Result<ObterLancamentoResponse?>.Success(response);
    }
}
