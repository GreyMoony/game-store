using System.Security.Claims;
using GameStore.Application.DTOs.UserDtos;
using GameStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Tests.TestUtilities.ServicesTestData;
public static class RoleServiceTestData
{
    public static IdentityRole Role => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = "Admin",
    };

    public static CreateRoleDto CreateRoleDto => new()
    {
        Role = new RoleShortDto { Name = "Manager" },
        Permissions =
        [
            "ManageUsers",
            "ViewReports",
        ],
    };

    public static IdentityRole AdminRole => new()
    {
        Id = "role-1",
        Name = "Admin",
    };

    public static IdentityRole ManagerRole => new()
    {
        Id = "role-2",
        Name = "Manager",
    };

    public static List<Claim> AdminClaims =>
    [
        new Claim("Permission", "ManageUsers"),
        new Claim("Permission", "ViewReports"),
    ];

    public static UpdateRoleDto UpdateRoleDto => new()
    {
        Role = new RoleDto { Id = "role-1", Name = "SuperAdmin" },
        Permissions =
        [
            "ManageUsers",
            "ManageRoles",
        ],
    };

    public static ApplicationUser UserWithRoles => new()
    {
        Id = "user-1",
        UserName = "UserWithRoles",
    };
}
