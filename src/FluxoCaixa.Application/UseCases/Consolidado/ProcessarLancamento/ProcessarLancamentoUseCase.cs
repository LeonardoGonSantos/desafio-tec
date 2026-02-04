using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Domain.Consolidado.Entities;
using FluxoCaixa.Shared.Interfaces;
using FluxoCaixa.Shared.Results;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Application.UseCases.Consolidado.ProcessarLancamento;

public interface IProcessarLancamentoUseCase : IUseCase<ProcessarLancamentoCommand, Result>
{
}

public class ProcessarLancamentoUseCase : IProcessarLancamentoUseCase
{
    private readonly IConsolidadoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessarLancamentoUseCase> _logger;

    public ProcessarLancamentoUseCase(
        IConsolidadoRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<ProcessarLancamentoUseCase> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Result>> ExecuteAsync(
        ProcessarLancamentoCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var data = command.DataLancamento.Date;

            var consolidado = await _repository.GetByDataAsync(data, cancellationToken);

            if (consolidado is null)
            {
                var criarResult = ConsolidadoDiario.Criar(data, 0);
                if (criarResult.IsFailure)
                    return Result<Result>.Failure(criarResult.Errors);

                consolidado = criarResult.Value!;
                await _repository.InsertAsync(consolidado, cancellationToken);
            }

            if (string.Equals(command.Tipo, "Credito", StringComparison.OrdinalIgnoreCase))
            {
                var result = consolidado.AdicionarCredito(command.Valor);
                if (result.IsFailure)
                    return Result<Result>.Failure(result.Errors);
            }
            else if (string.Equals(command.Tipo, "Debito", StringComparison.OrdinalIgnoreCase))
            {
                var result = consolidado.AdicionarDebito(command.Valor);
                if (result.IsFailure)
                    return Result<Result>.Failure(result.Errors);
            }
            else
            {
                return Result<Result>.Failure("Tipo de lançamento inválido. Use 'Debito' ou 'Credito'");
            }

            await _repository.UpdateAsync(consolidado, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Lançamento {LancamentoId} processado no consolidado da data {Data}", command.LancamentoId, data);

            return Result<Result>.Success(Result.Success());
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erro ao processar lançamento {LancamentoId}", command.LancamentoId);
            return Result<Result>.Failure($"Erro ao processar lançamento: {ex.Message}");
        }
    }
}
