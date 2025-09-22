namespace GameStore.Domain.Entities;
public class LocalizedGame
{
    public Guid Id { get; set; }

    public string Key { get; set; }

    public double Price { get; set; }

    public int UnitInStock { get; set; }

    public int Discount { get; set; }

    public Guid PublisherId { get; set; }

    public int ViewCount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? ImageName { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
