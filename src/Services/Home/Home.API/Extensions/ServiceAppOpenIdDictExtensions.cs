using Home.Domain.Identity;
using Home.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace Home.API.Extensions;

public static class ServiceAppOpenIdDictExtensions
{
    /// <summary>Register the entity sets needed by OpenIddict</summary>
    public static DbContextOptionsBuilder CustomizeOpenIdDictContext(this DbContextOptionsBuilder builder)
    {
        // Using the generic overload because I need to replace the default OpenIddict entities.
        builder.UseOpenIddict<AuthApplication, AuthAuthorization, AuthScope, AuthToken, string>();
        return builder;
    }

    /// <summary>Act as server</summary>
    public static IServiceCollection CustomizeOpenIdDictAsServer(this IServiceCollection services, IHostEnvironment environment, string apiName)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

        services.AddOpenIddict()
            .AddCore(opt =>
            {
                opt.UseEntityFrameworkCore()
                   .UseDbContext<HomeDbContext>();
            })
            .AddServer(opt =>
            {
                // Endpoints que daremos de alta
                opt.SetAuthorizationEndpointUris("/connect/authorize");
                opt.SetTokenEndpointUris("/connect/token");
                opt.SetLogoutEndpointUris("/connect/logout");
                opt.SetUserinfoEndpointUris("/connect/userinfo");

                // Registro de scopes que el servidor de OpenID conoce para todos los clientes.
                // Deben registrarse antes de configurar flujos (GitHub Issue #835)
                opt.RegisterScopes(
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Roles,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.OfflineAccess,
                    apiName);

                opt.AllowPasswordFlow(); // Auth server
                opt.AllowAuthorizationCodeFlow(); // SPA
                opt.AllowClientCredentialsFlow(); // Entre APIs
                opt.AllowRefreshTokenFlow(); // Para poder renovar el token en OfflineAccess
                opt.AllowImplicitFlow(); // Para swagger

                opt.UseReferenceAccessTokens(); // Guarda el token de acceso en BD cuando llenamos de demasiada información de claims. Si no va a ser así, mejor deshabilitarlo.
                opt.UseReferenceRefreshTokens(); // Guarda el token de refresco en BD cuando llenamos de demasiada información de claims. Si no va a ser así, mejor deshabilitarlo.

                // Lifetime de los tokens
                opt.SetAccessTokenLifetime(TimeSpan.FromMinutes(5));
                opt.SetRefreshTokenLifetime(TimeSpan.FromDays(1));

                // Para development
                if (environment.IsDevelopment())
                {
                    opt.AddDevelopmentEncryptionCertificate();
                    opt.AddDevelopmentSigningCertificate();
                }

                opt.UseAspNetCore()
                    //.DisableTransportSecurityRequirement() // Durante el desarrollo se puede deshabilitar Https
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableLogoutEndpointPassthrough()
                    .EnableUserinfoEndpointPassthrough()
                    .EnableStatusCodePagesIntegration();

                // Force client applications to use Proof Key for Code Exchange (PKCE).
                // Ojo, aplica a todos los clientes. Si es para un cliente concreto, aplicar directamente sobre él.
                // opt.RequireProofKeyForCodeExchange();

                // Note: if you don't want to specify a client_id when sending a token or revocation request, uncomment the following line:
                //
                // opt.AcceptAnonymousClients();

                // Note: if you want to process authorization and token requests that specify non-registered scopes, uncomment the following line:
                //
                // opt.DisableScopeValidation();

                // Note: if you don't want to use permissions, you can disable permission enforcement by uncommenting the following lines:
                //
                // opt.IgnoreEndpointPermissions()
                //        .IgnoreGrantTypePermissions()
                //        .IgnoreResponseTypePermissions()
                //        .IgnoreScopePermissions();

                // Note: when issuing access tokens used by third-party APIs you don't own, you can disable access token encryption:
                //
                // opt.DisableAccessTokenEncryption();
            })
            .AddValidation(opt =>
            {
                opt.UseLocalServer();
                opt.UseAspNetCore();
            });

        return services;
    }

    /// <summary>Setup Identity options with OpenIdDict</summary>
    public static IdentityOptions CustomizeOpenIdDictIdentityOptions(this IdentityOptions options)
    {
        // Mapeamos Identity para que use los nombres de los claims de OpenIDDict (aunque creo que son exactamente los mismos)
        options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Username;
        options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
        options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
        options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;

        return options;
    }

    /// <summary>Setup Identity</summary>
    public static IServiceCollection CustomizeIdentityServer(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<HomeDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager()
            .AddUserManager<UserManager<User>>()
            .AddRoleManager<RoleManager<Role>>();

        services.Configure<IdentityOptions>(opt =>
        {
            opt.Password.RequiredLength = 0;
            opt.Password.RequiredUniqueChars = 0;
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireUppercase = false;

            opt.SignIn.RequireConfirmedAccount = false;
            opt.SignIn.RequireConfirmedEmail = false;
            opt.SignIn.RequireConfirmedPhoneNumber = false;

            opt.CustomizeOpenIdDictIdentityOptions();
        });

        return services;
    }
}
