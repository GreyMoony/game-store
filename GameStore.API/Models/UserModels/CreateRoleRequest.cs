namespace GameStore.API.Models.UserModels;

public class CreateRoleRequest
{
    public RoleShortModel Role { get; set; }

    public IEnumerable<string> Permissions { get; set; }
}
