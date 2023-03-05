using Home.API.Extensions;

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
            var configuration = AppConfiguration.GetConfiguration<Program>();
            Log.Logger = LogConfiguration.CreateSerilog(configuration, typeof(Program).Assembly.GetName().Name);

            Log.Information("Configuring web host ({ApplicationContext})...", Namespace);

            var host = CreateHostBuilder(configuration, args).Build();

            Log.Information("Starting web host ({ApplicationContext})...", Namespace);

            host.Run();

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

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.CaptureStartupErrors(false);
                webBuilder.UseStartup<Startup>().ConfigureKestrel((context, options) => options.AddServerHeader = false);
                webBuilder.ConfigureAppConfiguration(x => x.AddConfiguration(configuration));
                webBuilder.ConfigureKestrel(o => o.AddServerHeader = false);
            })
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureLogging((hostingContext, logging) => logging.ClearProviders())
            .UseSerilog();
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