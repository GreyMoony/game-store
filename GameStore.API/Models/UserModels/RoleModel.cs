using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.UserModels;

public class RoleModel
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }
}
