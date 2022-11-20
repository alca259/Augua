namespace Augua.SharedKernel.WebApps;

public static class IApplicationBuilderExtensions
{
    public static IHostBuilder UseCustomSerilog(this IHostBuilder hostBuilder) =>
        hostBuilder.ConfigureLogging((hostingContext, logging) => logging.ClearProviders())
                   .UseSerilog((host, loggerConfiguration) =>
                        loggerConfiguration.MinimumLevel.Debug()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                            .Enrich.FromLogContext()
                            .WriteTo.Console(
                                outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                                theme: AnsiConsoleTheme.Literate)
                            .CreateLogger());
}