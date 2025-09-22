using System.Security.Claims;
using GameStore.Application.DTOs.UserDtos;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Tests.TestUtilities.ServicesTestData;
public static class UserIdentityServiceTestData
{
    public static ApplicationUser AdminUser => new()
    {
        Id = "user-1",
        UserName = "AdminUser",
    };

    public static ApplicationUser NormalUser => new()
    {
        Id = "user-2",
        UserName = "NormalUser",
    };

    public static ApplicationUser NonExistentUser => new()
    {
        Id = "invalid-user",
        UserName = "InvalidUser",
    };

    public static IdentityRole AdminRole => new()
    {
        Name = "Admin",
    };

    public static IdentityRole UserRole => new()
    {
        Name = "User",
    };

    public static IdentityRole GuestRole => new()
    {
        Name = "Guest",
    };

    public static List<Claim> AdminClaims =>
    [
        new Claim(Permissions.PermissionClaim, Permissions.ViewDeletedGames),
        new Claim(Permissions.PermissionClaim, Permissions.ManageEntities),
        new Claim(Permissions.PermissionClaim, Permissions.ViewStockGames),
        new Claim(Permissions.PermissionClaim, Permissions.ManageGameComments),
        new Claim(Permissions.PermissionClaim, Permissions.BanUsersFromComments),
        new Claim(Permissions.PermissionClaim, Permissions.CommentGame),
        new Claim(Permissions.PermissionClaim, Permissions.EditDeletedGames),
        new Claim(Permissions.PermissionClaim, Permissions.EditOrders),
        new Claim(Permissions.PermissionClaim, Permissions.ManageDeletedGameComments),
        new Claim(Permissions.PermissionClaim, Permissions.ManageUsersAndRoles),
        new Claim(Permissions.PermissionClaim, Permissions.ViewOrdersHistory),
    ];

    public static List<Claim> GuestClaims =>
    [
        new Claim(Permissions.PermissionClaim, Permissions.ReadOnlyAccess),
    ];

    public static LoginDto ValidLoginDto => new()
    {
        Login = "AdminUser",
        Password = "CorrectPassword",
        InternalAuth = true,
    };

    public static LoginDto ValidExternalLoginDto => new()
    {
        Login = "AdminUser",
        Password = "CorrectPassword",
        InternalAuth = false,
    };

    public static LoginDto InvalidLoginDto => new()
    {
        Login = "AdminUser",
        Password = "WrongPassword",
        InternalAuth = true,
    };

    public static LoginDto InvalidExternalLoginDto => new()
    {
        Login = "AdminUser",
        Password = "WrongPassword",
        InternalAuth = false,
    };

    public static AuthResult SuccessAuthentication => new()
    {
        Success = true,
        User = new()
        {
            Email = "Email",
            FirstName = "FirstName",
            LastName = "LastName",
        },
    };

    public static CreateUserDto CreateUserDto => new()
    {
        User = new UserShortDto { Name = "NewUser" },
        Password = "NewPassword123!",
        Roles =
        [
            "User",
        ],
    };

    public static CreateUserDto InvalidCreateUserDto => new()
    {
        User = new UserShortDto { Name = "InvalidUser" },
        Password = "Weak",
        Roles =
        [
            "User",
        ],
    };

    public static IdentityError[] IdentityErrors =>
    [
        new IdentityError { Description = "Password too weak" },
        new IdentityError { Description = "Username already taken" },
    ];

    public static UpdateUserDto UpdateUserDto => new()
    {
        User = new UserDto { Id = "user-1", Name = "UpdatedUser" },
        Password = "NewUpdatedPassword!",
        Roles =
        [
            "Admin",
        ],
    };

    public static IEnumerable<object[]> CheckAccessCases()
    {
        yield return new object[]
        {
            new CheckAccessDto { TargetId = string.Empty, TargetPage = Pages.Games },
            false,
            new List<UserRole>(),
            true,
        }; // Guest Access
        yield return new object[]
        {
            new CheckAccessDto { TargetId = string.Empty, TargetPage = Pages.Users },
            false, new List<UserRole> { Domain.Enums.UserRole.Admin },
            true,
        }; // Admin Access
        yield return new object[]
        {
            new CheckAccessDto { TargetId = Guid.NewGuid().ToString(), TargetPage = Pages.User },
            false, new List<UserRole> { Domain.Enums.UserRole.Admin },
            true,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = Guid.NewGuid().ToString(), TargetPage = Pages.User },
            false, new List<UserRole> { Domain.Enums.UserRole.User },
            false,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = string.Empty, TargetPage = Pages.Users },
            false,
            new List<UserRole> { Domain.Enums.UserRole.User },
            false,
        }; // No Admin Access
        yield return new object[]
        {
            new CheckAccessDto { TargetId = string.Empty, TargetPage = Pages.Orders },
            false,
            new List<UserRole> { Domain.Enums.UserRole.Manager },
            true,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = string.Empty, TargetPage = Pages.Buy },
            false,
            new List<UserRole> { Domain.Enums.UserRole.User },
            true,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = string.Empty, TargetPage = "InvalidPage" },
            false,
            new List<UserRole>(),
            false,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = "gameKey", TargetPage = Pages.Game },
            false,
            new List<UserRole>(),
            true,
        }; // Game Store Access
        yield return new object[]
        {
            new CheckAccessDto { TargetId = "123", TargetPage = Pages.Order },
            false,
            new List<UserRole> { Domain.Enums.UserRole.Manager },
            true,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = "123", TargetPage = Pages.Order },
            false,
            new List<UserRole> { Domain.Enums.UserRole.User },
            false,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = "123", TargetPage = "InvalidPage" },
            false,
            new List<UserRole>(),
            false,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = "123", TargetPage = Pages.Genre },
            false,
            new List<UserRole>(),
            true,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = "123", TargetPage = Pages.Publisher },
            false,
            new List<UserRole>(),
            true,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = Guid.NewGuid().ToString(), TargetPage = Pages.Genre },
            false,
            new List<UserRole>(),
            true,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = Guid.NewGuid().ToString(), TargetPage = Pages.Publisher },
            false,
            new List<UserRole>(),
            true,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = Guid.NewGuid().ToString(), TargetPage = Pages.Platform },
            false,
            new List<UserRole>(),
            true,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = Guid.NewGuid().ToString(), TargetPage = Pages.Order },
            false,
            new List<UserRole> { Domain.Enums.UserRole.Manager },
            true,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = Guid.NewGuid().ToString(), TargetPage = Pages.Order },
            false,
            new List<UserRole> { Domain.Enums.UserRole.User },
            false,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = Guid.NewGuid().ToString(), TargetPage = Pages.ReplyComment },
            false,
            new List<UserRole> { Domain.Enums.UserRole.User },
            true,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = Guid.NewGuid().ToString(), TargetPage = "InvalidPage" },
            false,
            new List<UserRole>(),
            false,
        };
        yield return new object[]
        {
            new CheckAccessDto { TargetId = "invalid", TargetPage = Pages.Game },
            false,
            new List<UserRole>(),
            false,
        }; // Invalid Game Key
    }
}
