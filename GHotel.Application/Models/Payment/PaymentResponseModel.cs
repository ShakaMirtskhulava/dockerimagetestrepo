using GHotel.Domain.Enums;

namespace GHotel.Application.Models.Payment;

#pragma warning disable CS8618
public class PaymentResponseModel
{
    public string Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatuss Status { get; set; }
    public Currency Currency { get; set; }
    public string? ApproveLink { get; set; }
}
