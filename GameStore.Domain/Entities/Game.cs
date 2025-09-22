using System.ComponentModel.DataAnnotations;

namespace GameStore.Domain.Entities;

public class Game : BaseEntity, ICommonGame
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Key { get; set; }

    public string? Description { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public int UnitInStock { get; set; }

    [Required]
    public int Discount { get; set; }

    public Guid PublisherId { get; set; }

    public int ViewCount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ProductID { get; set; }

    public string? QuantityPerUnit { get; set; }

    public int? UnitsOnOrder { get; set; }

    public int? ReorderLevel { get; set; }

    public bool? Discontinued { get; set; }

    public bool IsDeleted { get; set; }

    public string? ImageName { get; set; }

    public Publisher Publisher { get; set; }

    public ICollection<GameGenre> GameGenres { get; set; }

    public ICollection<GamePlatform> GamePlatforms { get; set; }

    public ICollection<OrderGame> OrderGames { get; set; }

    public ICollection<Comment> Comments { get; set; }

    public ICollection<GameTranslation> Translations { get; set; }

    public int CountComments()
    {
        return Comments?.Count ?? 0;
    }

    public override string ToString()
    {
        return $"Game : {Name}, Id: {Id}, Key: {Key}, Description: {Description}";
    }
}
