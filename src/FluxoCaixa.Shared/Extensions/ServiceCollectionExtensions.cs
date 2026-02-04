using FluxoCaixa.Shared.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace FluxoCaixa.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        return services;
    }
}
