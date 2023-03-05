using SharedKernel.Utils;
using Serilog.Context;
using System.Security.Claims;

namespace Home.API.Extensions;

public static class ServiceApplicationExtensions
{
    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app) => app
        .UseSwagger()
        .UseSwaggerUI(setup =>
        {
            setup.SwaggerEndpoint(string.Empty + "/swagger/v1/swagger.json", "Home API");
            setup.RoutePrefix = string.Empty;
            setup.OAuthAppName("Home API Swagger UI");

            setup.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        });

    public static IApplicationBuilder UseCustomSerilogRequestLogging(this IApplicationBuilder app) =>
            app.Use(async (context, next) =>
            {
                var requestID = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                var identity = context.User?.Identity as ClaimsIdentity;
                identity?.AddClaim(new Claim("RequestContext", requestID));

                using (LogContext.PushProperty("UserContext", context.User?.FindFirst("name")?.Value ?? ""))
                using (LogContext.PushProperty("ClientContext", context.User?.FindFirst("client_id")?.Value ?? ""))
                using (LogContext.PushProperty("RequestContext", context.User?.FindFirst("RequestContext")?.Value ?? ""))
                {
                    await next.Invoke();
                }
            })
            .UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HANDLED {IpAddress} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

                options.GetLevel = (httpContext, elapsed, ex) =>
                {
                    if (ex != null || httpContext.Response.StatusCode > 499)
                        return LogEventLevel.Error;
                    else if (elapsed < 100)
                        return LogEventLevel.Verbose;
                    else if (elapsed < 500)
                        return LogEventLevel.Debug;
                    else if (elapsed < 2000)
                        return LogEventLevel.Information;
                    else if (elapsed < 6000)
                        return LogEventLevel.Warning;
                    else
                        return LogEventLevel.Error;
                };

                options.EnrichDiagnosticContext = (diagnostic, context) =>
                {
                    diagnostic.Set("IpAddress", IPAddressHttpParser.GetIPAddress(context));
                };
            });
}
