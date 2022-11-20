using Augua.SharedKernel.WebApps;
using Serilog;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<WebAppsStartup>())
    .ConfigureLogging((hostingContext, logging) => logging.ClearProviders())
    .UseSerilog()
    .Build()
    .Run();