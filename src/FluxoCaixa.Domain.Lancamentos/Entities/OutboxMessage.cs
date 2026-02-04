namespace FluxoCaixa.Domain.Lancamentos.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public Guid AggregateId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? IntegrationStatus { get; set; }
    public DateTime? IntegrationAt { get; set; }
    public string? IntegrationError { get; set; }
    public int RetryCount { get; set; }

    private OutboxMessage() { }

    public static OutboxMessage Criar(Guid aggregateId, string eventType, string payload)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            AggregateId = aggregateId,
            EventType = eventType,
            Payload = payload,
            CreatedAt = DateTime.UtcNow,
            IntegrationStatus = "pending",
            RetryCount = 0
        };
    }
}
