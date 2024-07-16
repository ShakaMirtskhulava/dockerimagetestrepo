using GHotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GHotel.Persistance.Configurations.EntityConfigurations;

public class PaymentConfiguration : EntityConfiguration, IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.Property(x => x.Amount).IsRequired().HasPrecision(18, 2);

        builder.Property(x => x.Currency).IsRequired();
        builder.Property(x => x.Method).IsRequired();
        builder.Property(x => x.Status).IsRequired();
    }
}
