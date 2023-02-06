using System.Text;

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

    public static IApplicationBuilder UseCustomHeaders(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            var sb = new StringBuilder();
            sb.Append("default-src 'self' data: 'unsafe-inline' 'unsafe-eval' https://maxcdn.bootstrapcdn.com https://apis.google.com https://fonts.googleapis.com https://ajax.googleapis.com https://player.vimeo.com https://fonts.gstatic.com https://pro.fontawesome.com;");
            sb.Append(" font-src https://netdna.bootstrapcdn.com/font-awesome/;");
            sb.Append(" script-src 'self';");
            sb.Append(" connect-src 'self';");
            sb.Append(" img-src 'self';");
            sb.Append(" style-src 'self';");
            sb.Append(" base-uri 'self';");
            sb.Append(" form-action 'self'");

            context.Response.Headers.Add("X-XSS-Protection", "0");
            context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("Content-Security-Policy", sb.ToString());
            context.Response.Headers.Add("Referrer-Policy", "no-referrer");
            context.Response.Headers.Add("Permissions-Policy", "");
            context.Response.Headers.Add("Cross-Origin-Embedder-Policy", "require-corp");
            context.Response.Headers.Add("Cross-Origin-Resource-Policy", "same-origin");
            context.Response.Headers.Add("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
            await next.Invoke();
        });
}