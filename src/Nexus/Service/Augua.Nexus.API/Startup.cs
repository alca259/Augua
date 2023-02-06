using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Nexus.API.Extensions;
using System.Net;

namespace Nexus.API;

public sealed class Startup
{
    private readonly IConfiguration Configuration;
    private readonly IWebHostEnvironment Environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {

    }

    public void Configure(IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Remove("server");
            await next.Invoke();
        })
        .UseHttpsRedirection()
        .UseCors("AllowLocal")
        .UseProblemDetails()
        .Use(async (context, next) =>
        {
            context.Response.Headers.Remove("server");
            await next.Invoke();
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                int num = context.Response.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(new { Type = "https://httpstatuses.com/" + num, Status = num, Title = "Unauthorized", SeverityLevel = num }.ToJson());
            }
        })
        .UseCustomSwagger()
        .UseRouting()
        .UseAuthentication()
        .UseAuthorization()
        .UseCustomSerilogRequestLogging()
        .UseRequestLocalization() // Debe ir después de la autenticación y autorización, puesto que si no, los datos de context.User no están cargados
        .UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("default", "{controller=home}/{action=Get}").RequireAuthorization();
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions() { Predicate = r => r.Name.Contains("self") });
        });
    }
}
