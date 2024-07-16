using GHotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GHotel.Persistance.Configurations.EntityConfigurations;

public class RoomConfiguration : EntityConfiguration, IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Capacity).IsRequired();
        builder.Property(x => x.PricePerNight).IsRequired().HasPrecision(18, 2);
        builder.Property(x => x.PricePerNightCurrency).IsRequired();

        builder
            .HasMany(x => x.Images)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
