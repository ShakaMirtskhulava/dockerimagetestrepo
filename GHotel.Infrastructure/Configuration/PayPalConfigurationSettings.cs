namespace GHotel.Infrastructure.Configuration;

#pragma warning disable CS8618
public class PayPalConfigurationSettings
{
    public string ClientID { get; set; }
    public string Secret { get; set; }
    public string BaseUrl { get; set; }
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string Version { get; set; }
}
