using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.PlatformModels;

public class PlatformModel
{
    [Required(ErrorMessage = "Platform Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Platform type is required")]
    public string Type { get; set; }
}
