using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using DavidGroup.Core.Identity.Conventions;
using DavidGroup.Core.Identity.Data;
using DavidGroup.Core.Identity.Options;
using DavidGroup.Core.Identity.PolicyProviders;
using DavidGroup.Core.Identity.ResultHandlers;
using DavidGroup.Core.Identity.Swagger;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DavidGroup.Core.Identity.Extensions;

/// <summary>
/// Provides extension methods to configure JWT authentication and permission-based authorization.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures JWT authentication for the application using settings from configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add authentication to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing JWT settings.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/> for further authentication configuration.</returns>
    public static AuthenticationBuilder AddIdentityAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddIdentityAuthentication(configuration, []);
    }

    /// <summary>
    /// Configures JWT authentication for the application using settings from configuration
    /// and optionally enables token support for SignalR hubs.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add authentication to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing JWT settings.</param>
    /// <param name="hubs">An array of SignalR hub paths for which JWT tokens should be read from HTTP headers.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/> for further authentication configuration.</returns>
    public static AuthenticationBuilder AddIdentityAuthentication(this IServiceCollection services,
        IConfiguration configuration, string[] hubs)
    {
        // JWT Options
        services.AddOptions<JwtOptions>()
            .BindConfiguration(nameof(JwtOptions))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        JwtOptions jwtAuthorizationOptions = configuration
            .GetRequiredSection(nameof(JwtOptions))
            .Get<JwtOptions>()!;

        // Policy Providers & Handlers
        services.AddSingleton<IAuthorizationPolicyProvider, DefaultIdentityAuthorizationPolicyProvider>();

        services.Scan(scan => scan
            .FromAssemblies(typeof(ServiceCollectionExtensions).Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(AuthorizationHandler<>)))
            .As<IAuthorizationHandler>()
            .WithSingletonLifetime());

        // Result Handlers
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, DetailedAuthorizationResultHandler>();

        // Swagger Documentation
        services.ConfigureOptions<SwaggerPermissionsOptions>();

        return services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.MapInboundClaims = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtAuthorizationOptions.Issuer,
                ValidAudience = jwtAuthorizationOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthorizationOptions.SecretKey)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RoleClaimType = DavidGroupClaimTypes.Role,
                NameClaimType = DavidGroupClaimTypes.Nickname
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (hubs.Length == 0 || !context.Request.Headers.TryGetBearerToken(out string? bearerToken))
                        return Task.CompletedTask;

                    PathString path = context.HttpContext.Request.Path;
                    foreach (string hub in hubs)
                    {
                        if (!path.StartsWithSegments(hub))
                            continue;

                        context.Token = bearerToken;
                        break;
                    }

                    return Task.CompletedTask;
                },
                OnChallenge = async context =>
                {
                    context.HandleResponse();

                    ProblemDetails problemDetails = new()
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = "Unauthorized",
                        Detail = context.ErrorDescription ??
                                 context.AuthenticateFailure?.Message ??
                                 "You are not authorized to access this resource."
                    };

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/problem+json";

                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
                },
                OnTokenValidated = context =>
                {
                    string? typ = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Typ);
                    if (typ == DavidGroupTokenTypes.TwoFactorAuthentication)
                        context.Fail("Two factor authentication challenge token is not valid for API access.");

                    return Task.CompletedTask;
                }
            };
        });
    }

    /// <summary>
    /// Adds MVC controller services configured for Identity module.
    /// </summary>
    /// <param name="services">The service collection to add MVC services to.</param>
    /// <param name="setupAction">
    /// An action used to configure additional MVC options.
    /// </param>
    /// <returns>
    /// An <see cref="IMvcBuilder"/> that can be used to further configure MVC services.
    /// </returns>
    /// <remarks>
    /// This extension registers the <see cref="AuthorizationResponsesConvention"/> which
    /// automatically adds authorization-related response metadata to controller actions.
    /// </remarks>
    public static IMvcBuilder AddControllersWithIdentity(this IServiceCollection services,
        Action<MvcOptions>? setupAction = null)
    {
        return services.AddControllers(options =>
        {
            options.Conventions.Add(new AuthorizationResponsesConvention());

            setupAction?.Invoke(options);
        });
    }
}
