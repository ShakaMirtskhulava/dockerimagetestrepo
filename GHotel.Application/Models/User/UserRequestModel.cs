namespace GHotel.Application.Models.User;

#pragma warning disable CS8618
public class UserRequestModel
{
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? Password { get; set; }
}
