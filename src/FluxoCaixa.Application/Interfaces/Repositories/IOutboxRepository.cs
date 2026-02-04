using FluxoCaixa.Domain.Lancamentos.Entities;

namespace FluxoCaixa.Application.Interfaces.Repositories;

public interface IOutboxRepository
{
    Task InsertAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task<IEnumerable<OutboxMessage>> GetUnpublishedAsync(int batchSize, CancellationToken cancellationToken = default);
    Task MarkAsPublishedAsync(Guid id, CancellationToken cancellationToken = default);
    Task MarkIntegrationSuccessAsync(Guid id, CancellationToken cancellationToken = default);
    Task MarkIntegrationFailedAsync(Guid id, string error, CancellationToken cancellationToken = default);
}
