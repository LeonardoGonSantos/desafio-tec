using System.Text;
using System.Text.Json;
using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Application.UseCases.Consolidado.ProcessarLancamento;
using FluxoCaixa.Domain.Lancamentos.Events;
using FluxoCaixa.Infrastructure.Messaging;
using FluxoCaixa.Shared.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog.Context;

namespace FluxoCaixa.Worker.Services;

public class IntegrationWorkerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IntegrationWorkerService> _logger;
    private readonly RabbitMqConfiguration _config;
    private IConnection? _connection;
    private IModel? _channel;
    private const int MaxRetryCount = 5;

    public IntegrationWorkerService(
        IServiceProvider serviceProvider,
        RabbitMqConfiguration config,
        ILogger<IntegrationWorkerService> logger)
    {
        _serviceProvider = serviceProvider;
        _config = config;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        _logger.LogInformation("Iniciando Integration Worker Service");

        var factory = new ConnectionFactory
        {
            HostName = _config.HostName,
            Port = _config.Port,
            UserName = _config.UserName,
            Password = _config.Password,
            VirtualHost = _config.VirtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("lancamentos", ExchangeType.Topic, durable: true, autoDelete: false);

        var queueName = _channel.QueueDeclare("lancamentos.events", durable: true, exclusive: false, autoDelete: false).QueueName;
        _channel.QueueBind(queueName, "lancamentos", "LancamentoRegistradoEvent");
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) => await ProcessMessageAsync(ea, stoppingToken);

        _channel.BasicConsume(queueName, autoAck: false, consumer: consumer);

        _logger.LogInformation("Worker aguardando mensagens na queue: {QueueName}", queueName);

        return Task.CompletedTask;
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        var correlationId = ea.BasicProperties.CorrelationId ?? Guid.NewGuid().ToString();

        using var scope = _serviceProvider.CreateScope();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Mensagem recebida: {MessageId}", ea.BasicProperties.MessageId);

                var evento = JsonSerializer.Deserialize<LancamentoRegistradoEvent>(message);

                if (evento is null)
                {
                    _logger.LogWarning("Não foi possível deserializar o evento");
                    _channel?.BasicNack(ea.DeliveryTag, false, false);
                    return;
                }

                var outboxId = GetOutboxIdFromHeaders(ea.BasicProperties);
                var result = await ProcessIntegrationAsync(evento, outboxId, scope, cancellationToken);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Integração processada com sucesso: LancamentoId={LancamentoId}", evento.LancamentoId);
                    _channel?.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    _logger.LogError("Falha na integração: LancamentoId={LancamentoId}, Erros={Erros}",
                        evento.LancamentoId, string.Join(", ", result.Errors));

                    var retryCount = GetRetryCount(ea.BasicProperties);

                    if (retryCount < MaxRetryCount)
                    {
                        _logger.LogInformation("Reenfileirando para retry. Tentativa {Retry}/{Max}", retryCount + 1, MaxRetryCount);
                        _channel?.BasicNack(ea.DeliveryTag, false, true);
                    }
                    else
                    {
                        _logger.LogError("Máximo de retries atingido. Descartando mensagem.");
                        _channel?.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem");
                _channel?.BasicNack(ea.DeliveryTag, false, true);
            }
        }
    }

    private async Task<Result> ProcessIntegrationAsync(
        LancamentoRegistradoEvent evento,
        Guid? outboxId,
        IServiceScope scope,
        CancellationToken cancellationToken)
    {
        var useCase = scope.ServiceProvider.GetRequiredService<IProcessarLancamentoUseCase>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

        try
        {
            var command = new ProcessarLancamentoCommand(
                evento.LancamentoId,
                evento.Tipo,
                evento.Valor,
                evento.DataLancamento);

            var result = await useCase.ExecuteAsync(command, cancellationToken);

            if (outboxId.HasValue)
            {
                if (result.IsSuccess)
                    await outboxRepository.MarkIntegrationSuccessAsync(outboxId.Value, cancellationToken);
                else
                    await outboxRepository.MarkIntegrationFailedAsync(outboxId.Value, string.Join(", ", result.Errors), cancellationToken);
            }

            return result.IsSuccess ? Result.Success() : Result.Failure(result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção ao processar integração");
            if (outboxId.HasValue)
                await outboxRepository.MarkIntegrationFailedAsync(outboxId.Value, ex.Message, cancellationToken);
            return Result.Failure($"Erro na integração: {ex.Message}");
        }
    }

    private static Guid? GetOutboxIdFromHeaders(IBasicProperties properties)
    {
        if (properties.Headers == null || !properties.Headers.TryGetValue("OutboxId", out var value))
            return null;
        if (value is byte[] bytes)
        {
            var guidString = Encoding.UTF8.GetString(bytes);
            if (Guid.TryParse(guidString, out var guid))
                return guid;
        }
        return null;
    }

    private static int GetRetryCount(IBasicProperties properties)
    {
        if (properties.Headers != null && properties.Headers.TryGetValue("x-retry-count", out var value) && value is int count)
            return count;
        return 0;
    }

    public override void Dispose()
    {
        _logger.LogInformation("Finalizando Integration Worker Service");
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        base.Dispose();
    }
}
