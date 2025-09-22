using GameStore.API.Models.UserModels;
using GameStore.Application.DTOs.UserDtos;

namespace GameStore.Tests.TestUtilities.ControllersTestData;
public static class RolesControllerTestData
{
    public static List<RoleDto> RoleDtos =>
    [
        new()
        {
            Id = "adimnId",
            Name = "Admin",
        },
        new()
        {
            Id = "userId",
            Name = "User",
        },
    ];

    public static List<string> Permissions =>
    [
        "ViewUsers",
        "EditRoles",
    ];

    public static CreateRoleRequest CreateRoleRequest => new()
    {
        Role = new RoleShortModel { Name = "Manager" },
        Permissions = Permissions,
    };

    public static UpdateRoleRequest UpdateRoleRequest => new()
    {
        Role = new RoleModel { Id = Guid.NewGuid(), Name = "UpdatedManager" },
        Permissions = Permissions,
    };
}
