using FluxoCaixa.Domain.Consolidado.Entities;

namespace FluxoCaixa.Application.Interfaces.Repositories;

public interface IConsolidadoRepository
{
    Task<ConsolidadoDiario?> GetByDataAsync(DateTime data, CancellationToken cancellationToken = default);
    Task InsertAsync(ConsolidadoDiario consolidado, CancellationToken cancellationToken = default);
    Task UpdateAsync(ConsolidadoDiario consolidado, CancellationToken cancellationToken = default);
}
