using GHotel.Domain.Enums;

namespace GHotel.Application.Models.Payment;

#pragma warning disable CS8618
public class PaymentRequestModel
{
    public decimal Amount { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public PaymentMethod Method { get; set; } = PaymentMethod.Card;
    public PaymentStatuss Status { get; set; } = PaymentStatuss.Pending;
    public Currency Currency { get; set; } = Currency.USD;
}
