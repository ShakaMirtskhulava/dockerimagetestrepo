using System.Text.Json;
using GHotel.Application.Utilities;
using GHotel.Domain.Enums;
using GHotel.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace GHotel.Infrastructure.Utilities;

public class TBCCurrencyConvertion : ICurrencyConvertionUtility
{
    private readonly TBCCurrencyConvertionSettings _configurationSettings;

    public TBCCurrencyConvertion(IOptions<TBCCurrencyConvertionSettings> tBCCurrencyConvertionSettingsOptions)
    {
        _configurationSettings = tBCCurrencyConvertionSettingsOptions.Value;
    }

    public async Task<decimal> ConvertCurrency(decimal amount, Currency fromCurrency, Currency toCurrency,CancellationToken cancellationToken)
    {
        if (fromCurrency == toCurrency)
            return amount;

        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{_configurationSettings.BaseUrl}/{_configurationSettings.Version}/exchange-rates/nbg/convert?amount={amount}&from={fromCurrency}&to={toCurrency}"),
            Headers =
            {
                { "Accept", "application/json" },
                { "apikey", $"{_configurationSettings.APIKey}" },
            },
        };
        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var value = JsonDocument.Parse(body).RootElement.GetProperty("value").GetDecimal();
        return Math.Round(value, 2);
    }
}
