using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.GameModels;

public class GameModel
{
    [Required(ErrorMessage = "Game Id is required.")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Game name is required.")]
    public string Name { get; set; }

    public string? Key { get; set; }

    public string Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public double Price { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Unit in stock must be a positive number.")]
    public int UnitInStock { get; set; }

    [Required]
    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
    public int Discount { get; set; }
}
