using Dapper;
using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Domain.Lancamentos.Entities;
using FluxoCaixa.Shared.Interfaces;

namespace FluxoCaixa.Infrastructure.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public OutboxRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task InsertAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO outbox (id, aggregate_id, event_type, payload, created_at, integration_status, retry_count)
            VALUES (@Id, @AggregateId, @EventType, @Payload, @CreatedAt, 'pending', 0)";

        var conn = _unitOfWork.GetConnection();
        var trans = _unitOfWork.GetTransaction();

        await conn.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                message.Id,
                message.AggregateId,
                message.EventType,
                message.Payload,
                message.CreatedAt
            }, transaction: trans, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnpublishedAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT id, aggregate_id as AggregateId, event_type as EventType, payload, created_at as CreatedAt,
                   published_at as PublishedAt, integration_status as IntegrationStatus,
                   integration_at as IntegrationAt, integration_error as IntegrationError, retry_count as RetryCount
            FROM outbox
            WHERE published_at IS NULL
            ORDER BY created_at
            LIMIT @BatchSize";

        var conn = _unitOfWork.GetConnection();

        return await conn.QueryAsync<OutboxMessage>(
            new CommandDefinition(sql, new { BatchSize = batchSize }, cancellationToken: cancellationToken));
    }

    public async Task MarkAsPublishedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"UPDATE outbox SET published_at = @PublishedAt WHERE id = @Id";

        var conn = _unitOfWork.GetConnection();

        await conn.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id, PublishedAt = DateTime.UtcNow }, cancellationToken: cancellationToken));
    }

    public async Task MarkIntegrationSuccessAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE outbox
            SET integration_status = 'success', integration_at = @IntegrationAt
            WHERE id = @Id";

        var conn = _unitOfWork.GetConnection();

        await conn.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id, IntegrationAt = DateTime.UtcNow }, cancellationToken: cancellationToken));
    }

    public async Task MarkIntegrationFailedAsync(Guid id, string error, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE outbox
            SET integration_status = 'failed', integration_at = @IntegrationAt, integration_error = @Error, retry_count = retry_count + 1
            WHERE id = @Id";

        var conn = _unitOfWork.GetConnection();

        await conn.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id, IntegrationAt = DateTime.UtcNow, Error = error }, cancellationToken: cancellationToken));
    }
}
