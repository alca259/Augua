namespace Home.API.Extensions;

public static class AppConfiguration
{
    public static WebApplicationBuilder GetConfiguration<T>(this WebApplicationBuilder builder) where T : class
    {
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        if (isDevelopment)
            builder.Configuration.AddUserSecrets<T>();

        return builder;
    }
}
