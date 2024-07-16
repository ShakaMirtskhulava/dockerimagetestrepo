using GHotel.API.Infrastructure.Configuration;
using GHotel.Infrastructure.Configuration;

namespace GHotel.API.Infrastructure.Extensions;

public static class OptionsConfigurationExtensions
{
    public static IServiceCollection ConfigureOptions(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<FrontEndConfiguration>(configuration.GetSection(nameof(FrontEndConfiguration)));
        services.Configure<GoogleSSOConfiguration>(configuration.GetSection(nameof(GoogleSSOConfiguration)));
        services.Configure<EmailConfiguration>(configuration.GetSection("Logging").GetSection("Email"));

        return services;
    }
}
