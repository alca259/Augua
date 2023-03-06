using Home.API.Extensions;
using Home.Infrastructure.Data;

namespace Home.API;

public class Program
{
    public static readonly string Namespace = typeof(Program).Namespace;

    public static int Main(string[] args)
    {
        Console.Title = "Home API";
        EnsureEnviromentVariable();
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.GetConfiguration<Program>();

            Log.Logger = LogConfiguration.CreateSerilog(builder.Configuration, typeof(Program).Assembly.GetName().Name);
            Log.Information("Configuring web host ({ApplicationContext})...", Namespace);

            var startup = new Startup(builder.Configuration, builder.Environment);
            startup.ConfigureServices(builder.Services);

            builder.WebHost.CaptureStartupErrors(false);
            builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);
            builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
            builder.Host
                .ConfigureLogging((hostingContext, logging) => logging.ClearProviders())
                .UseSerilog();

            var app = builder.Build();
            
            app.MigrateDatabase<HomeDbContext>();

            startup.Configure(app, builder.Environment);

            Log.Information("Starting web host ({ApplicationContext})...", Namespace);

            app.Run();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", Namespace);
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void EnsureEnviromentVariable()
    {
#if DEBUG
        const string defaultEnvironmentName = "Development";
#else
        const string defaultEnvironmentName = "Production";
#endif
        // Para test unitarios y de aceptación, vienen sin environment, así que le establecemos uno
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.IsNullOrEmpty(envName)) Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", defaultEnvironmentName);
    }
}