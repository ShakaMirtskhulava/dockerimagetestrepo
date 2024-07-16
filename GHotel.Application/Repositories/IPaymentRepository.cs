using GHotel.Domain.Entities;

namespace GHotel.Application.Repositories;

public interface IPaymentRepository
{
    Task<Payment> Add(Payment payment, CancellationToken cancellationToken);
    Task<Payment?> Get(string id, CancellationToken cancellationToken);
    Task<Payment?> Update(Payment payment, CancellationToken cancellationToken);
}
