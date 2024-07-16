using GHotel.Domain.Enums;

namespace GHotel.Application.Utilities;

public interface ICurrencyConvertionUtility
{
    Task<decimal> ConvertCurrency(decimal amount, Currency fromCurrency, Currency toCurrency,CancellationToken cancellationToken);
}
