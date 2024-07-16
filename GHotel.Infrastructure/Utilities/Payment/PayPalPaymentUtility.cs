using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GHotel.Application.Models.Payment;
using GHotel.Application.Utilities;
using GHotel.Domain.Enums;
using GHotel.Infrastructure.Configuration;
using GHotel.Infrastructure.Exceptions;
using Microsoft.Extensions.Options;

namespace GHotel.Infrastructure.Utilities.Payment;

public class PayPalPaymentUtility : IPaymentUtility
{
    private readonly PayPalConfigurationSettings _payPalConfigurationSettings;
    private readonly HttpClient _httpClient;
    private readonly AppConfiguration _appConfiguration;

    public PayPalPaymentUtility(IOptions<PayPalConfigurationSettings> configurationSettings, IOptions<AppConfiguration> appConfiguration)
    {
        _httpClient = new HttpClient();
        _payPalConfigurationSettings = configurationSettings.Value;
        _appConfiguration = appConfiguration.Value;
    }

    public async Task<CreatePaymentResponse> CreatePayment(decimal amount, Currency currency, CancellationToken cancellationToken)
    {
        var feePercentage = 0.04m;
        var totalAmount = amount / (1 - feePercentage);
        var fee = totalAmount - amount;

        var url = $"{_payPalConfigurationSettings.BaseUrl}/{_payPalConfigurationSettings.Version}/checkout/orders";

        var order = new
        {
            intent = "AUTHORIZE",
            purchase_units = new[]
            {
                new
                {
                    amount = new
                    {
                        currency_code = currency.ToString(),
                        value = totalAmount.ToString("F2",CultureInfo.CurrentCulture),
                    }
                }
            },
            application_context = new
            {
                return_url = _appConfiguration.BaseUrl + _payPalConfigurationSettings.SuccessUrl,
                cancel_url = _appConfiguration.BaseUrl + _payPalConfigurationSettings.CancelUrl
            },
        };
        var ordersJson = JsonSerializer.Serialize(order);
        var requestContent = new StringContent(ordersJson, Encoding.UTF8, "application/json");

        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_payPalConfigurationSettings.ClientID}:{_payPalConfigurationSettings.Secret}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        var response = await _httpClient.PostAsync(url, requestContent, cancellationToken).ConfigureAwait(false);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var doc = JsonDocument.Parse(content);
            var payMentId = doc.RootElement.GetProperty("id").GetString();
            if (payMentId == null)
                throw new PaymentNotCreatedException("Couldn't make paypal payment, since the paymentId is null");

            var links = doc.RootElement.GetProperty("links");
            var approveLink = links.EnumerateArray().FirstOrDefault(l => l.GetProperty("rel").GetString() == "approve").GetProperty("href").GetString();
            if (approveLink == null)
                throw new PaymentNotCreatedException("Couldn't make paypal payment, since the approveLink is not found");
            return new CreatePaymentResponse
            {
                ApproveLink = approveLink,
                PaymentId = payMentId
            };
        }

        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        throw new PaymentNotCreatedException(jsonResponse);
    }

    public async Task<string> CheckoutAuthorizedPayment(string paymentId, CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessToken(cancellationToken).ConfigureAwait(false);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var requestUrl = $"{_payPalConfigurationSettings.BaseUrl}/{_payPalConfigurationSettings.Version}/checkout/orders/{paymentId}/authorize";

        var emptyJson = new StringContent("{}", Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(requestUrl, emptyJson, cancellationToken).ConfigureAwait(false);
        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to capture payment for order with paymentId {paymentId}. Response: {jsonResponse}");
        var doc = JsonDocument.Parse(jsonResponse);
        var authorizationId = doc.RootElement.GetProperty("purchase_units").EnumerateArray().First().GetProperty("payments").GetProperty("authorizations").EnumerateArray().First().GetProperty("id").GetString();
        if (authorizationId == null)
            throw new Exception("Authorization ID not found in the response.");
        return authorizationId;
    }

    public async Task CaptureAuthorizePayment(string authorizationId,decimal captureAmount,Currency currency,CancellationToken cancellationToken)
    {
        var feePercentage = 0.04m;
        var totalAmount = captureAmount / (1 - feePercentage);

        var accessToken = await GetAccessToken(cancellationToken).ConfigureAwait(false);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var requestUrl = $"{_payPalConfigurationSettings.BaseUrl}/{_payPalConfigurationSettings.Version}/payments/authorizations/{authorizationId}/capture";
        var captureRequest = new
        {
            amount = new
            {
                currency_code = currency.ToString(),
                value = totalAmount.ToString("F2", CultureInfo.CurrentCulture).Replace(',', '.')
            },
            is_final_capture = true
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(captureRequest), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUrl, requestContent, cancellationToken).ConfigureAwait(false);
        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to capture payment for authorization {authorizationId}. Response: {jsonResponse}");
    }

    private async Task<string?> GetAccessToken(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_payPalConfigurationSettings.BaseUrl}/v1/oauth2/token")
        {
            Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded")
        };
        var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_payPalConfigurationSettings.ClientID}:{_payPalConfigurationSettings.Secret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new PaymentNotCreatedException($"Failed to get access token. Response: {responseString}");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var tokenResponse = JsonSerializer.Deserialize<PayPalTokenResponse>(responseString, options);
        return tokenResponse?.AccessToken;
    }

}
