using System.Security.Claims;
using AutoMapper;
using GameStore.API.Models.UserModels;
using GameStore.Application.DTOs.UserDtos;
using GameStore.Application.Services.NotificationServices;
using GameStore.Application.Services.UserServices;
using GameStore.Domain.Constants;
using GameStore.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

[Route("api/")]
[ApiController]
public class UsersController(
    IUserService userService,
    INotificationService notificationService,
    IMapper mapper) : ControllerBase
{
    [HttpPost("users/login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest login)
    {
        var token = await userService.Login(mapper.Map<LoginDto>(login.Model));
        return Ok(new { Token = token });
    }

    [HttpPost("users/access")]
    public async Task<IActionResult> CheckAccess([FromBody] CheckAccessRequest check)
    {
        bool canSeeDeleted = User.Identity?.IsAuthenticated == true &&
                   User.Claims.Any(c => c.Type == Permissions.PermissionClaim &&
                                        c.Value == Permissions.ViewDeletedGames);
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => Enum.TryParse<UserRole>(c.Value, out var role) ? role : UserRole.Guest)
            .Distinct()
            .ToList();

        var result = await userService.CheckAccess(mapper.Map<CheckAccessDto>(check), canSeeDeleted, roles);

        return Ok(result);
    }

    [Authorize(Policy = Policies.CanManageUsersAndRoles)]
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userService.GetAllUsersAsync();

        return Ok(users);
    }

    [Authorize(Policy = Policies.CanManageUsersAndRoles)]
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await userService.GetUserByIdAsync(id);

        return Ok(user);
    }

    [Authorize(Policy = Policies.CanManageUsersAndRoles)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var result = await userService.DeleteUserByIdAsync(id);

        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [Authorize(Policy = Policies.CanManageUsersAndRoles)]
    [HttpPost("users")]
    public async Task<IActionResult> AddUser([FromBody] CreateUserRequest request)
    {
        var user = await userService.AddUserAsync(mapper.Map<CreateUserDto>(request));

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    [Authorize(Policy = Policies.CanManageUsersAndRoles)]
    [HttpPut("users")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        await userService.UpdateUserAsync(mapper.Map<UpdateUserDto>(request));

        return NoContent();
    }

    [HttpGet("users/notifications")]
    public IActionResult GetNotificationMethods()
    {
        return Ok(NotificationMethods.List);
    }

    [Authorize]
    [HttpGet("users/my/notifications")]
    public async Task<IActionResult> GetUserNotificationMethods()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        var methods = await notificationService.GetUserNotificationMethodsAsync(userId);

        return Ok(methods);
    }

    [Authorize]
    [HttpPut("users/notifications")]
    public async Task<IActionResult> UpdateUserMethods([FromBody] NotificationMethodsUpdateModel updateModel)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        await notificationService.UpdateUserNotificationMethodsAsync(userId, updateModel.Notifications);
        return NoContent();
    }
}
