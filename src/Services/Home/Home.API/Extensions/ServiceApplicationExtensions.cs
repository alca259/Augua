using SharedKernel.Utils;
using Serilog.Context;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace Home.API.Extensions;

public static class ServiceApplicationExtensions
{
    #region Services builder
    public static IServiceCollection CustomizeCors(this IServiceCollection services, string policyName, string authUri)
    {
        services.AddCors(opt =>
        {
            opt.AddPolicy(policyName, cfg => cfg
                .AllowCredentials()
                .WithOrigins(authUri, "http://localhost:4200")
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyHeader()
                .AllowAnyMethod());
        });
        return services;
    }

    public static IServiceCollection CustomizeSwagger(this IServiceCollection services, string authUri, string apiName, string apiAuthUrl = null, string apiTokenUrl = null)
    {
        var apiAuthority = authUri;
        var basePath = authUri;
        var scopeName = apiName;

        services.AddSwaggerGen(options =>
        {
            options.DescribeAllParametersInCamelCase();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = apiName,
                Version = "v1",
                Description = apiName + " HTTP",
                Contact = new OpenApiContact
                {
                    Name = "Foo Company",
                    Email = string.Empty,
                    Url = new Uri("https://foo.com/"),
                }
            });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = string.IsNullOrWhiteSpace(apiAuthUrl)
                            ? new Uri($"{apiAuthority}/connect/authorize")
                            : new Uri($"{apiAuthUrl}"),

                        TokenUrl = string.IsNullOrWhiteSpace(apiTokenUrl)
                            ? new Uri($"{apiAuthority}/connect/token")
                            : new Uri($"{apiTokenUrl}"),

                        Scopes = new Dictionary<string, string>
                        {
                            { scopeName, basePath }
                        }
                    }
                },
                Type = SecuritySchemeType.OAuth2,
                In = ParameterLocation.Header,
                Name = HeaderNames.Authorization

            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                    },
                    new[] { scopeName }
                }
            });
        });

        return services;
    }
    #endregion

    #region Middleware builder
    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, string apiName, string clientID, string clientSecret, string clientDisplayName) => app
        .UseSwagger()
        .UseSwaggerUI(setup =>
        {
            setup.SwaggerEndpoint(string.Empty + "/swagger/v1/swagger.json", apiName);
            setup.RoutePrefix = string.Empty;

            setup.OAuthAppName($"{clientDisplayName} Swagger UI");
            setup.OAuthClientId(clientID);
            setup.OAuthClientSecret(clientSecret);

            setup.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        });

    public static IApplicationBuilder UseCustomHeaderTreatment(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Remove("server");
            await next.Invoke();
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                int num = context.Response.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(new { Type = "https://httpstatuses.com/" + num, Status = num, Title = "Unauthorized", SeverityLevel = num }.ToJson());
            }
        });

    public static IApplicationBuilder UseCustomSerilogRequestLogging(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            var requestID = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var identity = context.User?.Identity as ClaimsIdentity;
            identity?.AddClaim(new Claim("RequestContext", requestID));

            using (LogContext.PushProperty("UserContext", context.User?.FindFirst("name")?.Value ?? ""))
            using (LogContext.PushProperty("ClientContext", context.User?.FindFirst("client_id")?.Value ?? ""))
            using (LogContext.PushProperty("RequestContext", context.User?.FindFirst("RequestContext")?.Value ?? ""))
            {
                await next.Invoke();
            }
        })
        .UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HANDLED {IpAddress} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (ex != null || httpContext.Response.StatusCode > 499)
                    return LogEventLevel.Error;
                else if (elapsed < 100)
                    return LogEventLevel.Verbose;
                else if (elapsed < 500)
                    return LogEventLevel.Debug;
                else if (elapsed < 2000)
                    return LogEventLevel.Information;
                else if (elapsed < 6000)
                    return LogEventLevel.Warning;
                else
                    return LogEventLevel.Error;
            };

            options.EnrichDiagnosticContext = (diagnostic, context) =>
            {
                diagnostic.Set("IpAddress", IPAddressHttpParser.GetIPAddress(context));
            };
        });
    #endregion
}
