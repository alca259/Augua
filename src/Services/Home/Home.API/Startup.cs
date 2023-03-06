using Hellang.Middleware.ProblemDetails;
using Home.API.Extensions;
using Home.API.Services;
using Home.Domain.Identity;
using Home.Infrastructure;
using Home.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;

namespace Home.API;

public sealed class Startup
{
    private const string CORS_POLICY = "AllowLocal";
    public const string API_NAME = "Home.API";
    public const string AUTH_URI = "https://localhost:5020";
    public const string CLIENT_ID = "home-api-client";
    public const string CLIENT_SECRET = "IGNORE_ME_TEST_SECRET";
    public const string CLIENT_DISPLAYNAME = "Home API";

    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddMvcCore().AddRazorViewEngine();
        services.AddProblemDetails();
        services.AddHealthChecks();

        services.AddDbContext<HomeDbContext>(opt =>
        {
            opt.UseSqlServer(_configuration.GetConnectionString("Default"), options =>
            {
                options.MigrationsAssembly(InfrastructureAssemblyReference.GetAssembly().FullName);
            });
            opt.CustomizeOpenIdDictContext();
        });

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

        services.CustomizeIdentityServer();

        services.CustomizeOpenIdDictAsServerAndClient(_environment, API_NAME, AUTH_URI, CLIENT_ID, CLIENT_SECRET, new HashSet<string> { API_NAME });
        services.CustomizeCors(CORS_POLICY, AUTH_URI);
        services.CustomizeSwagger(AUTH_URI, API_NAME);

        services.AddScoped<ILoginService<User>, LoginService>();
        services.AddHostedService<SeedInitTask>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            IdentityModelEventSource.ShowPII = true;
            app.UseDeveloperExceptionPage();
        }

        app
            .UseCors(CORS_POLICY)
            .UseHttpsRedirection()
            .UseStaticFiles()
            .UseProblemDetails()
            .UseCustomHeaderTreatment()
            .UseCustomSwagger(API_NAME, CLIENT_ID, CLIENT_SECRET, CLIENT_DISPLAYNAME)
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseCustomSerilogRequestLogging()
            .UseRequestLocalization(opt =>
            {
                // Debe ir después de la autenticación y autorización, puesto que si no, los datos de context.User no están cargados
                opt.AddSupportedCultures("es-ES");
                opt.AddSupportedUICultures("es-ES");
                opt.SetDefaultCulture("es-ES");
            })
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions() { Predicate = r => r.Name.Contains("self") });
            });
    }
}
