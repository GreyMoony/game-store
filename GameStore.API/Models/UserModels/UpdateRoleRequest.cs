namespace GameStore.API.Models.UserModels;

public class UpdateRoleRequest
{
    public RoleModel Role { get; set; }

    public IEnumerable<string> Permissions { get; set; }
}
