namespace GHotel.Domain.Entities;

#pragma warning disable CS8618
public class MyRole : Entity
{
    public string Name { get; set; }
    public ICollection<MyUser>? Users { get; set; }
}
