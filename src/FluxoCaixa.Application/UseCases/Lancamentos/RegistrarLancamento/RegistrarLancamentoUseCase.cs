using System.Text.Json;
using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Domain.Lancamentos.Entities;
using FluxoCaixa.Domain.Lancamentos.Events;
using FluxoCaixa.Domain.Lancamentos.ValueObjects;
using FluxoCaixa.Shared.Interfaces;
using FluxoCaixa.Shared.Results;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Application.UseCases.Lancamentos.RegistrarLancamento;

public interface IRegistrarLancamentoUseCase : IUseCase<RegistrarLancamentoCommand, RegistrarLancamentoResponse>
{
}

public class RegistrarLancamentoUseCase : IRegistrarLancamentoUseCase
{
    private readonly ILancamentoRepository _repository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegistrarLancamentoUseCase> _logger;

    public RegistrarLancamentoUseCase(
        ILancamentoRepository repository,
        IOutboxRepository outboxRepository,
        IUnitOfWork unitOfWork,
        ILogger<RegistrarLancamentoUseCase> logger)
    {
        _repository = repository;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RegistrarLancamentoResponse>> ExecuteAsync(
        RegistrarLancamentoCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando registro de lançamento: {Tipo} - {Valor}", command.Tipo, command.Valor);

        if (!Enum.TryParse<TipoLancamento>(command.Tipo, true, out var tipo))
        {
            return Result<RegistrarLancamentoResponse>.Failure(
                "Tipo de lançamento inválido. Use 'Debito' ou 'Credito'");
        }

        var lancamentoResult = Lancamento.Criar(
            tipo,
            command.Valor,
            command.Descricao,
            command.DataLancamento);

        if (lancamentoResult.IsFailure)
            return Result<RegistrarLancamentoResponse>.Failure(lancamentoResult.Errors);

        var lancamento = lancamentoResult.Value!;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _repository.InsertAsync(lancamento, cancellationToken);

            var evento = lancamento.GerarEvento();
            var outboxMessage = OutboxMessage.Criar(
                lancamento.Id,
                nameof(LancamentoRegistradoEvent),
                JsonSerializer.Serialize(evento));

            await _outboxRepository.InsertAsync(outboxMessage, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Lançamento registrado com sucesso: {Id}", lancamento.Id);

            return Result<RegistrarLancamentoResponse>.Success(
                new RegistrarLancamentoResponse(lancamento.Id, lancamento.DataLancamento));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erro ao registrar lançamento");
            return Result<RegistrarLancamentoResponse>.Failure("Erro ao registrar lançamento");
        }
    }
}
