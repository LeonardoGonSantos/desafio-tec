using FluxoCaixa.Shared.Logging;
using FluxoCaixa.Worker.Extensions;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

SerilogConfiguration.ConfigureSerilog("FluxoCaixa.Worker");
builder.Services.AddSerilog();

builder.Services.AddWorkerServices(builder.Configuration);

var host = builder.Build();

try
{
    Log.Information("Iniciando FluxoCaixa.Worker");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker falhou ao iniciar");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
