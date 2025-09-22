using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.UserModels;

public class LoginModel
{
    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }

    public bool InternalAuth { get; set; }
}