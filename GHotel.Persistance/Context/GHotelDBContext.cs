using GHotel.Domain.Entities;
using GHotel.Persistance.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GHotel.Persistance.Context;

#pragma warning disable CS8618
public class GHotelDBContext : DbContext
{
    private readonly ConnectionStrings _connectionStrings;
    private readonly UpdateEntitiesInterceptor _updateEntitiesInterceptor;

    public DbSet<MyUser> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<MyImage> Images { get; set; }
    public DbSet<MyRole> Roles { get; set; }

    public GHotelDBContext(IOptions<ConnectionStrings> connectionStringsOptions, UpdateEntitiesInterceptor updateEntitiesInterceptor)
    {
        _connectionStrings = connectionStringsOptions.Value;
        _updateEntitiesInterceptor = updateEntitiesInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(_connectionStrings.DefaultConnection)
            .AddInterceptors(_updateEntitiesInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GHotelDBContext).Assembly);
    }

}
