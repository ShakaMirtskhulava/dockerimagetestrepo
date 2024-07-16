using GHotel.Persistance.Context;

namespace GHotel.Infrastructure.UOW;
public abstract class UnitOfWork : IDisposable
{
    private readonly GHotelDBContext _dbContext;

    protected UnitOfWork(GHotelDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
    
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
