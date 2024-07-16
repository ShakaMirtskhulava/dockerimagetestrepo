using GHotel.Application.Mapping;

namespace GHotel.API.Infrastructure.Mapping;
public static class MappingConfiguration
{
    public static void ConfigureMapping(this IServiceCollection services)
    {
        MapsterConfiguration.RegisterMaps();
        ConfigureMappingForUser();
    }

    public static void ConfigureMappingForUser()
    {
    }
}
