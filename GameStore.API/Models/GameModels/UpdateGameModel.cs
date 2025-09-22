using System.ComponentModel.DataAnnotations;
using GameStore.API.Attributes;

namespace GameStore.API.Models.GameModels;

public class UpdateGameModel
{
    [Required]
    public GameModel Game { get; set; }

    [Required]
    public IEnumerable<string> Genres { get; set; }

    [Required]
    public IEnumerable<Guid> Platforms { get; set; }

    [Required]
    public string Publisher { get; set; }

    [Base64Image]
    public string Image { get; set; }
}
