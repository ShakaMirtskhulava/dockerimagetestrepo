using GHotel.Domain.Entities;
using GHotel.Persistance.Configurations.OptionsConfigurations;
using GHotel.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GHotel.Persistance;

public static class Seed
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbContext>>();
        try
        {
            await Migrate(scope).ConfigureAwait(false);
            await SeedEverything(scope).ConfigureAwait(false);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        }
    }

    private static async Task Migrate(IServiceScope serviceScope)
    {
        var gHotelDBContext = serviceScope.ServiceProvider.GetRequiredService<GHotelDBContext>();
        await gHotelDBContext.Database.MigrateAsync().ConfigureAwait(false);
    }

    private static async Task SeedEverything(IServiceScope serviceScope)
    {
        await SeedRoles(serviceScope).ConfigureAwait(false);
        await SeedAdmin(serviceScope).ConfigureAwait(false);
    }

    private static async Task SeedRoles(IServiceScope serviceScope)
    {
        var gHotelDBContext = serviceScope.ServiceProvider.GetRequiredService<GHotelDBContext>();
        if (!gHotelDBContext.Roles.Any())
        {
            var roles = new List<MyRole>
            {
                new MyRole { Name = "Admin" },
                new MyRole { Name = "User" }
            };
            await gHotelDBContext.Roles.AddRangeAsync(roles).ConfigureAwait(false);
        }
        await gHotelDBContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private static async Task SeedAdmin(IServiceScope serviceScope)
    {
        var gHotelDBContext = serviceScope.ServiceProvider.GetRequiredService<GHotelDBContext>();
        var adminCredentials = serviceScope.ServiceProvider.GetRequiredService<IOptions<AdminCredentials>>().Value;
        if (!gHotelDBContext.Users.Any())
        {
            var adminRole = await gHotelDBContext.Roles.FirstAsync(r => r.Name == "Admin").ConfigureAwait(false);
            var admin = new MyUser
            {
                Email = adminCredentials.Email,
                EmailConfirmed = true,
                Roles = new List<MyRole>() { adminRole }
            };
            await gHotelDBContext.Users.AddAsync(admin).ConfigureAwait(false);

        }
        await gHotelDBContext.SaveChangesAsync().ConfigureAwait(false);
    }

}
