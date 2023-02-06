namespace Nexus.API.Extensions;

public sealed class AppConfiguration
{
    public static IConfiguration GetConfiguration<T>() where T : class
    {
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        var buildconfig = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        if (isDevelopment)
            buildconfig.AddUserSecrets<T>();

        var config = buildconfig.Build();

        var appconfiguration = config.GetConnectionString("AppConfiguration");

        if (appconfiguration.IsNullOrWhiteSpace())
            return config;

        buildconfig = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        if (isDevelopment)
            buildconfig.AddUserSecrets<T>();

        return buildconfig.Build();
    }
}
