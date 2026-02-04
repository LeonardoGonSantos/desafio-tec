using FluxoCaixa.Domain.Lancamentos.Entities;

namespace FluxoCaixa.Application.Interfaces.Repositories;

public interface ILancamentoRepository
{
    Task InsertAsync(Lancamento lancamento, CancellationToken cancellationToken = default);
    Task<Lancamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lancamento>> ListAsync(
        DateTime? dataInicio,
        DateTime? dataFim,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
