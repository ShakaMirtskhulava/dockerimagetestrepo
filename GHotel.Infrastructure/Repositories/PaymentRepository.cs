using GHotel.Application.Repositories;
using GHotel.Domain.Entities;
using GHotel.Persistance.Context;

namespace GHotel.Infrastructure.Repositories;

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(GHotelDBContext dbContext) : base(dbContext)
    {
    }

    public async new Task<Payment> Add(Payment payment, CancellationToken cancellationToken)
    {
        return await base.Add(payment, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Payment?> Get(string id,CancellationToken cancellationToken)
    {
        return await base.Get(cancellationToken, id).ConfigureAwait(false);
    }

    public async new Task<Payment?> Update(Payment payment, CancellationToken cancellationToken)
    {
        return await base.Update(payment, cancellationToken).ConfigureAwait(false);
    }

}
