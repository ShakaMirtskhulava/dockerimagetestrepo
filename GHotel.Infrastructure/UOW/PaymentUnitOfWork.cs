using GHotel.Application.Repositories;
using GHotel.Application.UOW;
using GHotel.Persistance.Context;

namespace GHotel.Infrastructure.UOW;

public class PaymentUnitOfWork : UnitOfWork, IPaymentUnitOfWork
{
    public IPaymentRepository PaymentRepository { get; }

    public PaymentUnitOfWork(GHotelDBContext dbContext, IPaymentRepository paymentRepository) : base(dbContext)
    {
        PaymentRepository = paymentRepository;
    }
}
