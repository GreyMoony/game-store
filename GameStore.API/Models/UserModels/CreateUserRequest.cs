using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.UserModels;

public class CreateUserRequest
{
    public UserShortModel User { get; set; }

    public IEnumerable<string> Roles { get; set; }

    [Required]
    public string Password { get; set; }
}
