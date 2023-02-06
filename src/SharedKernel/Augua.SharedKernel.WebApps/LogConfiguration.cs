namespace Augua.SharedKernel.WebApps;

public static class LogConfiguration
{
    public static ILogger CreateSerilog(IConfiguration configuration, string Namespace)
    {
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var template = "[{Timestamp:HH:mm:ss.fff} {Level:w3}] [{UserContext}-{ApplicationContext}] {Message}{NewLine}{Exception}";
        var loggerConf = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.WithProperty("ApplicationContext", Namespace)
            .Enrich.FromLogContext();

        if (envName == "Development")
            loggerConf.WriteTo.Console(outputTemplate: template);

        if (envName != "Development")
            loggerConf.WriteTo.File(configuration["Logging:Path"], outputTemplate: template);

        return loggerConf
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }
}