using GHotel.Domain.Enums;

namespace GHotel.Domain.Entities;

public abstract class Entity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public EntityStatus EntitiyStatus { get; set; } = EntityStatus.Active;
}
