using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.PlatformModels;

public class AddPlatformModel
{
    [Required(ErrorMessage = "Platform type is required")]
    public string Type { get; set; }
}
