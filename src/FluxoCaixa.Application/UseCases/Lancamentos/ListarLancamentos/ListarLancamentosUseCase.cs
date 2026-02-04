using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Domain.Lancamentos.Entities;
using FluxoCaixa.Domain.Lancamentos.ValueObjects;
using FluxoCaixa.Shared.Interfaces;
using FluxoCaixa.Shared.Results;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Application.UseCases.Lancamentos.ListarLancamentos;

public interface IListarLancamentosUseCase : IUseCase<ListarLancamentosQuery, ListarLancamentosResponse>
{
}

public class ListarLancamentosUseCase : IListarLancamentosUseCase
{
    private readonly ILancamentoRepository _repository;
    private readonly ILogger<ListarLancamentosUseCase> _logger;

    public ListarLancamentosUseCase(
        ILancamentoRepository repository,
        ILogger<ListarLancamentosUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ListarLancamentosResponse>> ExecuteAsync(
        ListarLancamentosQuery query,
        CancellationToken cancellationToken = default)
    {
        var lancamentos = await _repository.ListAsync(
            query.DataInicio,
            query.DataFim,
            query.Page,
            query.PageSize,
            cancellationToken);

        var list = lancamentos.ToList();
        var items = list.Select(l => new LancamentoItemResponse(
            l.Id,
            l.Tipo.ToString(),
            l.Valor.ValorDecimal,
            l.Descricao,
            l.DataLancamento,
            l.CreatedAt)).ToList();

        return Result<ListarLancamentosResponse>.Success(
            new ListarLancamentosResponse(items, query.Page, query.PageSize, list.Count));
    }
}
