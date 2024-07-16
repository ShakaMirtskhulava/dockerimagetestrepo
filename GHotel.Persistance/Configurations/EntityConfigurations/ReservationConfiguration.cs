using GHotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GHotel.Persistance.Configurations.EntityConfigurations;

public class ReservationConfiguration : EntityConfiguration, IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.Property(x => x.CheckInDateUtc).IsRequired();
        builder.Property(x => x.CheckOutDateUtc).IsRequired();
        builder.Property(x => x.NumberOfGuests).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Identifier).ValueGeneratedOnAdd();

        builder
            .HasOne(x => x.User)
            .WithMany(us => us.Reservations)
            .HasForeignKey(res => res.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(x => x.Room)
            .WithMany(ro => ro.Reservations)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(re => re.Payment)
            .WithOne(pa => pa.Reservation)
            .HasForeignKey<Reservation>(re => re.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
