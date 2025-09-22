using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Domain.Entities;
public class GamePlatform
{
    [Column(Order = 1)]
    public Guid GameId { get; set; }

    [Column(Order = 2)]
    public Guid PlatformId { get; set; }

    public Game Game { get; set; }

    public Platform Platform { get; set; }
}
