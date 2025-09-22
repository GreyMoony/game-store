namespace GameStore.API.Models.UserModels;

public class UpdateUserRequest
{
    public UserModel User { get; set; }

    public IEnumerable<string> Roles { get; set; }

    public string Password { get; set; }
}
