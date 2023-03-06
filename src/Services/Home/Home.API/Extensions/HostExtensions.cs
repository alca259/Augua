using Microsoft.EntityFrameworkCore;

namespace Home.API.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDatabase<TContext>(this IHost host) where TContext : DbContext
    {
        CreateScopeAndMigrate<TContext>(host.Services).Wait();
        return host;
    }

    private static async Task CreateScopeAndMigrate<TContext>(IServiceProvider serviceProvider) where TContext : DbContext
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        try
        {
            await (scope.ServiceProvider.GetService<TContext>()?.Database.MigrateAsync());
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"An error ocurred while migrating database for {typeof(TContext).FullName}");
            throw;
        }
    }
}