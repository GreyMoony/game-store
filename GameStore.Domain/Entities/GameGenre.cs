using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Domain.Entities;
public class GameGenre
{
    [Column(Order = 1)]
    public Guid GameId { get; set; }

    [Column(Order = 2)]
    public Guid GenreId { get; set; }

    public Game Game { get; set; }

    public Genre Genre { get; set; }
}
