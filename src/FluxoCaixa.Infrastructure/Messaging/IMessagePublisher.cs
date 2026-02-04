namespace FluxoCaixa.Infrastructure.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync(
        string exchange,
        string routingKey,
        string message,
        Guid? outboxId = null,
        CancellationToken cancellationToken = default);
}
