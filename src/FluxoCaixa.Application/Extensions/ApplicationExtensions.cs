using FluxoCaixa.Application.UseCases.Consolidado.ObterConsolidadoDiario;
using FluxoCaixa.Application.UseCases.Consolidado.ProcessarLancamento;
using FluxoCaixa.Application.UseCases.Lancamentos.ListarLancamentos;
using FluxoCaixa.Application.UseCases.Lancamentos.ObterLancamento;
using FluxoCaixa.Application.UseCases.Lancamentos.RegistrarLancamento;
using Microsoft.Extensions.DependencyInjection;

namespace FluxoCaixa.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IRegistrarLancamentoUseCase, RegistrarLancamentoUseCase>();
        services.AddScoped<IListarLancamentosUseCase, ListarLancamentosUseCase>();
        services.AddScoped<IObterLancamentoUseCase, ObterLancamentoUseCase>();
        services.AddScoped<IObterConsolidadoDiarioUseCase, ObterConsolidadoDiarioUseCase>();
        services.AddScoped<IProcessarLancamentoUseCase, ProcessarLancamentoUseCase>();
        return services;
    }
}
