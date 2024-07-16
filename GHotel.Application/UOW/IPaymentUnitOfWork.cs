using GHotel.Application.Repositories;

namespace GHotel.Application.UOW;

public interface IPaymentUnitOfWork : IUnitOfWork
{
    public IPaymentRepository PaymentRepository { get; }
}
