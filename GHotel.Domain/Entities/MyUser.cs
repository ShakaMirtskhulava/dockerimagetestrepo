namespace GHotel.Domain.Entities;

#pragma warning disable CS8618
public class MyUser : Entity
{
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PasswordHashe { get; set; }

    public ICollection<MyRole> Roles { get; set; }
    public ICollection<Reservation>? Reservations { get; set; }
}
