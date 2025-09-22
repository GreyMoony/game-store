using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameStore.Application.DTOs.UserDtos;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Enums;
using GameStore.Domain.Exceptions;
using GameStore.Domain.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GameStore.Application.Services.UserServices;

#pragma warning disable IDE0046 // Convert to conditional expression
public class UserIdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IExternalAuthService authService,
    IOptions<JwtSettings> jwtSettings,
    IUnitOfWork unitOfWork,
    IUnitOfWorkMongo unitOfWorkNorthwind) : IUserService
{
    public async Task<string> Login(LoginDto loginDto)
    {
        if (loginDto.InternalAuth)
        {
            var user = await userManager.FindByNameAsync(loginDto.Login);
            if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var token = await GenerateJwtTokenAsync(user);
            return token;
        }
        else
        {
            var result = await authService.AuthenticateAsync(loginDto);
            if (!result.Success)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var token = await GenerateJwtTokenForExternalUserAsync(result.User!);
            return token;
        }
    }

    public async Task<bool> CheckAccess(CheckAccessDto check, bool includeDeleted, List<UserRole> userRoles)
    {
        if (check.TargetId.IsNullOrEmpty())
        {
            return CheckPageAccess(check.TargetPage, userRoles);
        }
        else if (Guid.TryParse(check.TargetId, out var guidId))
        {
            return await CheckAccessByGuid(guidId, check.TargetPage, includeDeleted, userRoles);
        }
        else if (int.TryParse(check.TargetId, out var intId))
        {
            return await CheckAccessByInt(intId, check.TargetPage, includeDeleted, userRoles);
        }
        else
        {
            return await CheckAccessByString(check.TargetId, check.TargetPage, includeDeleted, userRoles);
        }
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await userManager.Users
            .Select(u => new UserDto { Id = u.Id, Name = u.UserName! })
            .ToListAsync();

        return users;
    }

    public async Task<UserDto> GetUserByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id) ??
            throw new EntityNotFoundException($"User with id {id} not found");

        return new UserDto { Id = user.Id, Name = user.UserName! };
    }

    public async Task<IdentityResult> DeleteUserByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id) ??
            throw new EntityNotFoundException($"User with id {id} not found");

        return await userManager.DeleteAsync(user);
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        var user = new ApplicationUser { UserName = request.User.Name };
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new EntityCreationException($"User {request.User.Name} was not created. Errors: {errors}");
        }

        await AddUserToRolesAsync(user, request.Roles);

        return new UserDto { Id = user.Id, Name = user.UserName };
    }

    public async Task UpdateUserAsync(UpdateUserDto request)
    {
        var user = await userManager.FindByIdAsync(request.User.Id) ??
            throw new EntityNotFoundException($"User with id {request.User.Id} not found");

        await UpdateUserNameAsync(user, request.User.Name);

        // Update Password (Only if provided)
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            await UpdateUserPassword(user, request.Password);
        }

        // Update Roles (Remove old roles & assign new ones)
        await UpdateUserRolesAsync(user, request.Roles);

        await userManager.UpdateAsync(user);
    }

    private async Task UpdateUserNameAsync(ApplicationUser user, string name)
    {
        if (user.UserName != name)
        {
            var result = await userManager.SetUserNameAsync(user, name);

            if (!result.Succeeded)
            {
                throw new UserUpdateException(
                    "Failed to update username: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private async Task UpdateUserPassword(ApplicationUser user, string password)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var passwordResult = await userManager.ResetPasswordAsync(user, token, password);

        if (!passwordResult.Succeeded)
        {
            throw new UserUpdateException(
                "Failed to update password: " +
                string.Join(", ", passwordResult.Errors.Select(e => e.Description)));
        }
    }

    private async Task AddUserToRolesAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        foreach (var roleId in roles)
        {
            var role = await roleManager.FindByIdAsync(roleId) ??
                throw new IdsNotValidException($"Role with Id {roleId} doesn't exist");
            if (role.Name != null)
            {
                await userManager.AddToRoleAsync(user, role.Name);
            }
        }
    }

    private async Task UpdateUserRolesAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);

        await AddUserToRolesAsync(user, roles);
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var claims = new HashSet<Claim>(new ClaimComparer())
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
        };

        // Add Role Claims
        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));

            var roleObj = await roleManager.FindByNameAsync(role);
            if (roleObj != null)
            {
                var roleClaims = await roleManager.GetClaimsAsync(roleObj);
                foreach (var claim in roleClaims)
                {
                    claims.Add(claim);
                }
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateJwtTokenForExternalUserAsync(AuthSuccessResponse user)
    {
        var claims = new HashSet<Claim>(new ClaimComparer())
    {
        new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Role, UserRoles.Guest),
    };

        // Add Guest Role Claims
        var roleObj = await roleManager.FindByNameAsync(UserRoles.Guest);
        if (roleObj != null)
        {
            var roleClaims = await roleManager.GetClaimsAsync(roleObj);
            foreach (var claim in roleClaims)
            {
                claims.Add(claim);
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<bool> CheckAccessToNorthwindAsync(int id, string page, bool includeDeleted)
    {
        NorthwindEntity entity = page switch
        {
            _ when page == Pages.Genre || Pages.ManagerGenrePages.Contains(page)
                => await unitOfWorkNorthwind.Categories.GetByIdAsync(id, includeDeleted),

            _ when page == Pages.Publisher || Pages.ManagerPublisherPages.Contains(page)
                => await unitOfWorkNorthwind.Suppliers.GetByIdAsync(id, includeDeleted),

            _ when Pages.ManagerOrderPages.Contains(page)
            => await unitOfWorkNorthwind.Orders.GetByIdAsync(id),

            _ => null,
        };

        return entity is not null;
    }

    private async Task<bool> CheckAccessToGameStoreAsync(Guid id, string page, bool includeDeleted)
    {
        BaseEntity entity = page switch
        {
            _ when page == Pages.Genre || Pages.ManagerGenrePages.Contains(page)
                => await unitOfWork.Genres.GetByIdAsync(id, includeDeleted),

            _ when page == Pages.Publisher || Pages.ManagerPublisherPages.Contains(page)
                => await unitOfWork.Publishers.GetByIdAsync(id, includeDeleted),

            _ when page == Pages.Platform || Pages.ManagerPlatformPages.Contains(page)
                => await unitOfWork.Platforms.GetByIdAsync(id, includeDeleted),

            _ when Pages.ManagerOrderPages.Contains(page)
                => await unitOfWork.Orders.GetByIdAsync(id),

            _ when Pages.UserCommentPages.Contains(page)
                => await unitOfWork.Comments.GetByIdAsync(id),

            _ => null,
        };

        return entity is not null;
    }

    private async Task<bool> CheckAccessToGameAsync(string key, bool includeDeleted)
    {
        var gameTask = unitOfWork.Games.GetByKeyAsync(key, includeDeleted);
        var productTask = unitOfWorkNorthwind.Products.GetByKeyAsync(key, includeDeleted);

        await Task.WhenAll(gameTask, productTask);

        return gameTask.Result is not null || productTask.Result is not null;
    }

    private async Task<bool> CheckAccessByGuid(Guid guidId, string targetPage, bool includeDeleted, List<UserRole> userRoles)
    {
        if (Pages.AdminPages.Contains(targetPage))
        {
            return userRoles.Exists(r => r >= UserRole.Admin);
        }

        if ((Pages.ManagerPages.Contains(targetPage) &&
                userRoles.Exists(r => r >= UserRole.Manager)) ||
                (Pages.ModeratorPages.Contains(targetPage) &&
                userRoles.Exists(r => r >= UserRole.Moderator)) ||
                (!Pages.ManagerPages.Contains(targetPage) &&
                !Pages.ModeratorPages.Contains(targetPage)))
        {
            return await CheckAccessToGameStoreAsync(guidId, targetPage, includeDeleted);
        }

        return false;
    }

    private async Task<bool> CheckAccessByInt(int intId, string targetPage, bool includeDeleted, List<UserRole> userRoles)
    {
        if ((Pages.ManagerPages.Contains(targetPage) &&
                userRoles.Exists(r => r >= UserRole.Manager)) ||
                !Pages.ManagerPages.Contains(targetPage))
        {
            return await CheckAccessToNorthwindAsync(intId, targetPage, includeDeleted);
        }

        return false;
    }

    private async Task<bool> CheckAccessByString(
        string targetId, string targetPage, bool includeDeleted, List<UserRole> userRoles)
    {
        if (!await CheckAccessToGameAsync(targetId, includeDeleted))
        {
            return false;
        }

        return (Pages.UserPages.Contains(targetPage) &&
                userRoles.Exists(r => r >= UserRole.User)) ||
               (Pages.ManagerPages.Contains(targetPage) &&
                userRoles.Exists(r => r >= UserRole.Manager)) ||
               (!Pages.UserPages.Contains(targetPage) &&
                !Pages.ManagerPages.Contains(targetPage));
    }

    private static bool CheckPageAccess(string targetPage, List<UserRole> userRoles)
    {
        return targetPage switch
        {
            _ when Pages.GuestPages.Contains(targetPage) => true,
            _ when Pages.AdminPages.Contains(targetPage) => userRoles.Exists(r => r >= UserRole.Admin),
            _ when Pages.ManagerPages.Contains(targetPage) => userRoles.Exists(r => r >= UserRole.Manager),
            _ when Pages.UserPages.Contains(targetPage) => userRoles.Exists(r => r >= UserRole.User),
            _ => false,
        };
    }

    private sealed class ClaimComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim? x, Claim? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Type == y.Type && x.Value == y.Value;
        }

        public int GetHashCode(Claim obj) => HashCode.Combine(obj.Type, obj.Value);
    }
}
#pragma warning restore IDE0046 // Convert to conditional expression
