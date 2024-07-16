using System.Security.Claims;
using GHotel.API.Infrastructure.Authentication;
using GHotel.API.Infrastructure.Authentication.Google;
using GHotel.API.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace GHotel.API.Infrastructure.Extensions;

public static class AuthenticationConfigurationExtensions
{
    public static void ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ISignInManager, SignInManager>();

        using var scope = services.BuildServiceProvider().CreateScope();
        var googleSSOConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<GoogleSSOConfiguration>>().Value;

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromDays(10);
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            })
            .AddGoogle(GoogleAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = googleSSOConfiguration.ClientID;
                options.ClientSecret = googleSSOConfiguration.ClientSecret;
            });
    }

    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("NotAuthenticated", policy =>
            {
                policy.RequireAssertion(context =>
                {
                    return !context.User.Identities.Any(x => x.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme);
                });
            });
            options.AddPolicy("Authenticated", policy =>
            {
                policy.RequireAssertion(context =>
                {
                    return context.User.Identities.Any(x => x.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme);
                });
            });
            options.AddPolicy("IsUser", policy =>
            {
                policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireClaim(ClaimTypes.Role, "User");
            });
            options.AddPolicy("IsAdmin", policy =>
            {
                policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireClaim(ClaimTypes.Role, "Admin");
            });

        });
    }

}
