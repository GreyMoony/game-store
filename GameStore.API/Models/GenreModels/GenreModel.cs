using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.GenreModels;

public class GenreModel
{
    [Required(ErrorMessage = "Genre Id is required")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Genre name is required")]
    public string Name { get; set; }

    public string? ParentGenreId { get; set; }
}
