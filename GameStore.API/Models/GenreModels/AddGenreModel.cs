using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.GenreModels;

public class AddGenreModel
{
    [Required(ErrorMessage = "Genre name is required")]
    public string Name { get; set; }

    public string? ParentGenreId { get; set; }
}
