namespace GHotel.API.Infrastructure.Configuration;

#nullable disable
public class FrontEndConfiguration
{
    public string Origin { get; set; }
    public string GoogleResponsePage { get; set; }
    public string ReservationSuccessPage { get; set; }
    public string ReservationCancelationPage { get; set; }
    public string ReservationErrorPage { get; set; }
    public string EmailConfirmationPage {get;set;}
}
