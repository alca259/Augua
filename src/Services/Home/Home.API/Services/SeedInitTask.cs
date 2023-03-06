using Home.Domain.Identity;
using Home.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using System.Globalization;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Home.API.Services;

public sealed class SeedInitTask : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public SeedInitTask(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var ctx = scope.ServiceProvider.GetRequiredService<HomeDbContext>();
        await ctx.Database.EnsureCreatedAsync(cancellationToken);

        await CreateSwaggerClient(scope, cancellationToken);
        //await CreateAngularClient(scope, cancellationToken);
        await RegisterScopes(scope, cancellationToken);
        await CreateDefaultUser(scope);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task CreateSwaggerClient(IServiceScope scope, CancellationToken cancellationToken)
    {
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var existDefaultAppClient = await manager.FindByClientIdAsync(Startup.CLIENT_ID, cancellationToken);
        if (existDefaultAppClient != null) return;

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = Startup.CLIENT_ID,
            ClientSecret = Startup.CLIENT_SECRET,
            DisplayName = Startup.CLIENT_DISPLAYNAME,
            ConsentType = ConsentTypes.Implicit,
            RedirectUris =
            {
                new Uri($"{Startup.AUTH_URI}/swagger/oauth2-redirect.html"),
                new Uri($"{Startup.AUTH_URI}/signin-oidc"),
                new Uri($"{Startup.AUTH_URI}/signout-oidc")
            },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Logout,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
                Permissions.GrantTypes.Implicit,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Token,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                $"{Permissions.Prefixes.Scope}{Startup.API_NAME}"
            }
        }, cancellationToken);
    }

    /*
    private static async Task CreateAngularClient(IServiceScope scope, CancellationToken cancellationToken)
    {
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var existDefaultAppClient = await manager.FindByClientIdAsync("angular-client", cancellationToken);
        if (existDefaultAppClient != null) return;

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "angular-client",
            ClientSecret = "D1D312D8-1FEB-4D9B-B0EB-169E73F0987B",
            DisplayName = "Auth Server Angular Client",
            ConsentType = ConsentTypes.Explicit,
            PostLogoutRedirectUris =
            {
                new Uri("http://localhost:6001/signout-oidc"),
                new Uri("http://localhost:6001")
            },
            RedirectUris =
            {
                new Uri("http://localhost:6001/signin-oidc"),
                new Uri("http://localhost:6001")
            },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Logout,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                $"{Permissions.Prefixes.Scope}API"
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }
        }, cancellationToken);
    }
    */
    private static async Task RegisterScopes(IServiceScope scope, CancellationToken cancellationToken)
    {
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (await manager.FindByNameAsync(Startup.API_NAME, cancellationToken) is null)
        {
            await manager.CreateAsync(new OpenIddictScopeDescriptor
            {
                DisplayName = $"{Startup.CLIENT_DISPLAYNAME} {Startup.API_NAME} scope",
                DisplayNames =
                {
                    [CultureInfo.GetCultureInfo("es-ES")] = $"Acceso de {Startup.CLIENT_DISPLAYNAME} a {Startup.API_NAME}"
                },
                Name = Startup.API_NAME,
                Resources =
                {
                    Startup.CLIENT_ID
                }
            }, cancellationToken);
        }
    }

    private static async Task CreateDefaultUser(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var user = new User
        {
            UserName = "administrator",
            FkUserRoles = new List<UserRole>
            {
                new UserRole
                {
                    FkRole = new Role
                    {
                        Name = "admin",
                        NormalizedName = "ADMIN"
                    }
                }
            }
        };

        var existUser = await userManager.FindByNameAsync(user.UserName);
        if (existUser != null) return;

        var pass = userManager.PasswordHasher.HashPassword(user, "demo");
        user.PasswordHash = pass;
        await userManager.CreateAsync(user);
    }
}
