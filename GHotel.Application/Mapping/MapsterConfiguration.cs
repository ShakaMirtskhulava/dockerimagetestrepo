using GHotel.Application.Models.User;
using GHotel.Domain.Entities;
using Mapster;

namespace GHotel.Application.Mapping;

public static class MapsterConfiguration
{
    public static void RegisterMaps()
    {
        ConfigureUserMapping();
    }

    private static void ConfigureUserMapping()
    {
        TypeAdapterConfig<MyUser, UserResponseModel>
            .NewConfig()
            .Map(urm => urm.Roles, us => GetRoles(us));
    }

    private static List<string>? GetRoles(MyUser user)
    {
        if (user.Roles == null || user.Roles.Count <= 0)
            return null;
        return user.Roles.Select(ro => ro.Name).ToList();
    }

}
