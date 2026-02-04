using FluxoCaixa.API.Extensions;
using FluxoCaixa.Application.Extensions;
using FluxoCaixa.Infrastructure.Extensions;
using FluxoCaixa.Shared.Extensions;
using FluxoCaixa.Shared.Logging;
using FluxoCaixa.Shared.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilogConfiguration("FluxoCaixa.API");

builder.Services
    .AddApiServices(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddSharedServices();

var app = builder.Build();

app.UseExceptionHandling();
app.UseCorrelationId();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "fluxocaixa-api" }));

try
{
    Log.Information("Iniciando FluxoCaixa.API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}
