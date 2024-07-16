using GHotel.Domain.Enums;

namespace GHotel.Domain.Entities;

#pragma warning disable CS8618
public class Payment : Entity
{
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatuss Status { get; set; }
    public Currency Currency { get; set; }
    public string? AuthorizationId { get; set; }

    public Reservation Reservation { get; set; }
}
