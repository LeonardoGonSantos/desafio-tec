using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Infrastructure.BackgroundServices;

public class OutboxPublisherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisherService> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

    public OutboxPublisherService(
        IServiceProvider serviceProvider,
        ILogger<OutboxPublisherService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Publisher Service iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar outbox");
            }

            await Task.Delay(Interval, stoppingToken);
        }

        _logger.LogInformation("Outbox Publisher Service finalizado");
    }

    private async Task ProcessOutboxAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

        var messages = await outboxRepository.GetUnpublishedAsync(100, cancellationToken);
        var list = messages.ToList();

        if (list.Count == 0)
            return;

        _logger.LogInformation("Processando {Count} mensagens do outbox", list.Count);

        foreach (var message in list)
        {
            try
            {
                await publisher.PublishAsync(
                    "lancamentos",
                    message.EventType,
                    message.Payload,
                    message.Id,
                    cancellationToken);

                await outboxRepository.MarkAsPublishedAsync(message.Id, cancellationToken);

                _logger.LogInformation("Mensagem {Id} publicada: {EventType}", message.Id, message.EventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar mensagem {Id}: {EventType}", message.Id, message.EventType);
            }
        }
    }
}
