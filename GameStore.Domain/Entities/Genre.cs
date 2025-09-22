using System.ComponentModel.DataAnnotations;

namespace GameStore.Domain.Entities;
public class Genre : BaseEntity
{
    [Required]
    public string Name { get; set; }

    public Guid? ParentGenreId { get; set; }

    public Genre ParentGenre { get; set; }

    public int? CategoryID { get; set; }

    public string? Description { get; set; }

    public string? Picture { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<GameGenre> GameGenres { get; set; }

    public ICollection<Genre> SubGenres { get; set; }
}
