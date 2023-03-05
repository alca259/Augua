using Hellang.Middleware.ProblemDetails;
using Home.API.Extensions;
using Home.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;

namespace Home.API;

public sealed class Startup
{
    private const string CORS_POLICY = "AllowLocal";
    private const string API_NAME = "Home.API";

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

        services.AddDbContext<HomeDbContext>(opt =>
        {
            opt.UseSqlServer(_configuration.GetConnectionString("Default"));
            opt.CustomizeOpenIdDictContext();
        });

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

        services.CustomizeIdentityServer();
        services.CustomizeOpenIdDictAsServer(_environment, API_NAME);
        services.CustomizeCors(CORS_POLICY);

        /* TODO: inicializar y controladores
        services.AddScoped<ILoginService<User>, LoginService>();
        services.AddHostedService<SeedInitTask>();
        */
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
            .UseCustomSwagger()
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
