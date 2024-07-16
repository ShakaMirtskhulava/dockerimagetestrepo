using GHotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GHotel.Persistance.Configurations.EntityConfigurations;

public class MyRoleConfiguration : EntityConfiguration,IEntityTypeConfiguration<MyRole>
{
    public void Configure(EntityTypeBuilder<MyRole> builder)
    {
        builder.ToTable("Roles");

        builder.HasIndex(ro => ro.Name).IsUnique();

        builder
            .HasMany(ro => ro.Users)
            .WithMany(us => us.Roles)
            .UsingEntity(j => j.ToTable("UserRoles"));

    }
}
