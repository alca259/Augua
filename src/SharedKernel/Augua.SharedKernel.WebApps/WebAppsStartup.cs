namespace Augua.SharedKernel.WebApps;

public class WebAppsStartup
{
    public WebAppsStartup(IConfiguration configuration)
    {
        Configuration = configuration;

        Log.Logger = LogConfiguration.CreateSerilog(configuration, Configuration["ClientID"]);
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<AppSettings>(Configuration);
        services.AddControllersWithViews();
        services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = "ClientApp/dist";
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSpaStaticFiles();
        if (!env.IsDevelopment())
        {
            app.UseSpaStaticFiles();
        }
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
        });

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp";
            spa.Options.StartupTimeout = new TimeSpan(0, 5, 0);

            if (env.IsDevelopment())
            {
                spa.UseAngularCliServer(npmScript: "start");
            }
        });
    }
}
