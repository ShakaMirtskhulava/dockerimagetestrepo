namespace GHotel.Infrastructure.Exceptions;

#pragma warning disable CS8618

public class PaymentNotCreatedException : Exception
{
    public string Code { get; }
    public PaymentNotCreatedException(string message) : base(message)
    {
    }
}
