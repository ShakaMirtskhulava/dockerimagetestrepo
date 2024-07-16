using GHotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GHotel.Persistance.Interceptors;

public class UpdateEntitiesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? dbContext = eventData.Context;
        if (dbContext == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var auditableEntities = dbContext.ChangeTracker.Entries<Entity>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

        foreach (var auditableEntity in auditableEntities)
        {
            if (auditableEntity.State == EntityState.Added)
                auditableEntity.Property(entity => entity.CreatedAtUtc).CurrentValue = DateTime.UtcNow;
            else if (auditableEntity.State == EntityState.Modified)
                auditableEntity.Property(entity => entity.UpdatedAtUtc).CurrentValue = DateTime.UtcNow;
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

}
