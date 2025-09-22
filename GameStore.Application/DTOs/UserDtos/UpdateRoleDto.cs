namespace GameStore.Application.DTOs.UserDtos;
public class UpdateRoleDto
{
    public RoleDto Role { get; set; }

    public IEnumerable<string> Permissions { get; set; }
}
