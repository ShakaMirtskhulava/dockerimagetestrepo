using GHotel.Application.Models.Payment;
using GHotel.Domain.Enums;

namespace GHotel.Application.Utilities;

public interface IPaymentUtility
{
    Task<CreatePaymentResponse> CreatePayment(decimal amount, Currency currency, CancellationToken cancellationToken);
    Task<string> CheckoutAuthorizedPayment(string paymentId, CancellationToken cancellationToken);
    Task CaptureAuthorizePayment(string authorizationId, decimal captureAmount, Currency currency, CancellationToken cancellationToken);
}
