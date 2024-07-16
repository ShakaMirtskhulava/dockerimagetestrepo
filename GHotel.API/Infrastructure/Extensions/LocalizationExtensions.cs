using GHotel.API.Infrastructure.Configuration;
using GHotel.API.Infrastructure.Middlewares;

namespace GHotel.API.Infrastructure.Extensions;

public static class LocalizationExtensions
{
    public static void ConfigureLocalization(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LocalizationConfiguration>(configuration.GetSection("Localization"));
    }

    public static IApplicationBuilder UseLocalization(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseMiddleware<LocalizationMiddleware>();
        return applicationBuilder;
    }
}
