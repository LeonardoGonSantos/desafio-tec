using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Infrastructure.BackgroundServices;
using FluxoCaixa.Infrastructure.Database;
using FluxoCaixa.Infrastructure.Messaging;
using FluxoCaixa.Infrastructure.Repositories;
using FluxoCaixa.Shared.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluxoCaixa.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não configurada.");

        services.AddScoped<IFluxoCaixaDbConnection>(_ => new FluxoCaixaDbConnection(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ILancamentoRepository, LancamentoRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IConsolidadoRepository, ConsolidadoRepository>();

        var rabbitConfig = configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>()
            ?? throw new InvalidOperationException("Configuração RabbitMq não encontrada.");
        services.AddSingleton(rabbitConfig);
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

        services.AddHostedService<OutboxPublisherService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServicesForWorker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não configurada.");

        services.AddScoped<IFluxoCaixaDbConnection>(_ => new FluxoCaixaDbConnection(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ILancamentoRepository, LancamentoRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IConsolidadoRepository, ConsolidadoRepository>();

        var rabbitConfig = configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>()
            ?? throw new InvalidOperationException("Configuração RabbitMq não encontrada.");
        services.AddSingleton(rabbitConfig);

        return services;
    }
}
