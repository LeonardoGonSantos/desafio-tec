using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Shared.Interfaces;
using FluxoCaixa.Shared.Results;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Application.UseCases.Consolidado.ObterConsolidadoDiario;

public interface IObterConsolidadoDiarioUseCase : IUseCase<ObterConsolidadoDiarioQuery, ObterConsolidadoDiarioResponse?>
{
}

public class ObterConsolidadoDiarioUseCase : IObterConsolidadoDiarioUseCase
{
    private readonly IConsolidadoRepository _repository;
    private readonly ILogger<ObterConsolidadoDiarioUseCase> _logger;

    public ObterConsolidadoDiarioUseCase(
        IConsolidadoRepository repository,
        ILogger<ObterConsolidadoDiarioUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ObterConsolidadoDiarioResponse?>> ExecuteAsync(
        ObterConsolidadoDiarioQuery query,
        CancellationToken cancellationToken = default)
    {
        var data = query.Data.Date;
        var consolidado = await _repository.GetByDataAsync(data, cancellationToken);

        if (consolidado is null)
            return Result<ObterConsolidadoDiarioResponse?>.Failure("Consolidado não encontrado para a data informada");

        var response = new ObterConsolidadoDiarioResponse(
            consolidado.Data.Valor,
            consolidado.Saldo.Valor,
            consolidado.TotalCreditos,
            consolidado.TotalDebitos,
            consolidado.QuantidadeLancamentos);

        return Result<ObterConsolidadoDiarioResponse?>.Success(response);
    }
}
