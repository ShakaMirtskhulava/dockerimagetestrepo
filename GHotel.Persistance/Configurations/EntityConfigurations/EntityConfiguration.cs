using GHotel.Domain.Entities;
using GHotel.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GHotel.Persistance.Configurations.EntityConfigurations;

public class EntityConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.EntitiyStatus).IsRequired();

        builder.HasQueryFilter(x => x.EntitiyStatus != EntityStatus.Deleted);
    }
}
