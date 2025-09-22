using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.UserModels;

public class RoleShortModel
{
    [Required]
    public string Name { get; set; }
}
