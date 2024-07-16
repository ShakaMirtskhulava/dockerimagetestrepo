using GHotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GHotel.Persistance.Configurations.EntityConfigurations;

public class MyImageConfiguration : EntityConfiguration, IEntityTypeConfiguration<MyImage>
{
    public void Configure(EntityTypeBuilder<MyImage> builder)
    {
        builder.ToTable("Images");
        builder.Property(x => x.Url).IsRequired();
    }
}
