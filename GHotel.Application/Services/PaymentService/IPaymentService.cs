using GHotel.Application.Models.Payment;

namespace GHotel.Application.Services.PaymentService;

public interface IPaymentService
{
    Task<PaymentResponseModel> CreatePayment(PaymentRequestModel paymentRequestModel, CancellationToken cancellationToken);
    Task<PaymentResponseModel> CheckoutAuthorizePayment(string paymentId, CancellationToken cancellationToken);
    Task<PaymentResponseModel> CaptureAuthorizePayment(string id, CancellationToken cancellationToken);
    Task<PaymentResponseModel> CancelPayment(string id, decimal fee, CancellationToken cancellationToken);
}
