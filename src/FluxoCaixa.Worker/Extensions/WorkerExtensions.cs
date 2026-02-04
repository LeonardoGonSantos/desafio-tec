using FluxoCaixa.Application.Extensions;
using FluxoCaixa.Infrastructure.Extensions;
using FluxoCaixa.Shared.Extensions;
using FluxoCaixa.Worker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluxoCaixa.Worker.Extensions;

public static class WorkerExtensions
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServicesForWorker(configuration);
        services.AddSharedServices();
        services.AddHostedService<IntegrationWorkerService>();
        return services;
    }
}
