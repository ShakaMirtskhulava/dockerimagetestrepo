using System.Text.Json.Serialization;

namespace GHotel.Infrastructure.Utilities.Payment;

public class PayPalTokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
}
