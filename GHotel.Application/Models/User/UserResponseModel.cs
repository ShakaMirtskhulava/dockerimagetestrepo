namespace GHotel.Application.Models.User;

#pragma warning disable CS8618
public class UserResponseModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public List<string>? Roles { get; set; }
}
