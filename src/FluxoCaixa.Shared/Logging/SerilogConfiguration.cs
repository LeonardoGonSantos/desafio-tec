using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace FluxoCaixa.Shared.Logging;

public static class SerilogConfiguration
{
    public static void ConfigureSerilog(string applicationName)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static IHostBuilder UseSerilogConfiguration(
        this IHostBuilder hostBuilder,
        string applicationName)
    {
        ConfigureSerilog(applicationName);
        return hostBuilder.UseSerilog();
    }
}
