namespace GameStore.Application.DTOs.UserDtos;
public class CreateRoleDto
{
    public RoleShortDto Role { get; set; }

    public IEnumerable<string> Permissions { get; set; }
}
