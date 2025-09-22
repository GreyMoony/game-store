using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.UserModels;

public class UserShortModel
{
    [Required]
    public string Name { get; set; }
}
