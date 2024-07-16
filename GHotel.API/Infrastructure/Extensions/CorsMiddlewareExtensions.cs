using GHotel.API.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace GHotel.API.Infrastructure.Extensions;

public static class CorsMiddlewareExtensions
{
    public static IApplicationBuilder UseCORS(this IApplicationBuilder app, IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var frontEndConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<FrontEndConfiguration>>().Value;
        var googleConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<GoogleSSOConfiguration>>().Value;

        app.UseCors(options =>
        {
            options
                .WithOrigins(frontEndConfiguration.Origin, googleConfiguration.Origin, "http://127.0.0.1:5500")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });

        return app;
    }
}
