namespace GHotel.Infrastructure.Configuration;

#pragma warning disable CS8618
public class EmailConfiguration
{
    public string From { get; set; }
    public string To { get; set; }
    public string Port { get; set; }
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Key { get; set; }
}
