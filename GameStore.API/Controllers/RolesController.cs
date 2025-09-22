using AutoMapper;
using GameStore.API.Models.UserModels;
using GameStore.Application.DTOs.UserDtos;
using GameStore.Application.Services.UserServices;
using GameStore.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

[Authorize(Policy = Policies.CanManageUsersAndRoles)]
[Route("api/")]
[ApiController]
public class RolesController(
    IRoleService roleService,
    IMapper mapper) : ControllerBase
{
    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await roleService.GetAllAsync();

        return Ok(roles);
    }

    [HttpGet("roles/{id}")]
    public async Task<IActionResult> GetRoleById(string id)
    {
        var role = await roleService.GetByIdAsync(id);

        return Ok(role);
    }

    [HttpDelete("roles/{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var result = await roleService.DeleteRoleAsync(id);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpGet("users/{id}/roles")]
    public async Task<IActionResult> GetUserRoles(string id)
    {
        var roles = await roleService.GetUsersRolesAsync(id);

        return Ok(roles);
    }

    [HttpGet("roles/permissions")]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await roleService.GetAllPermissionsAsync();

        return Ok(permissions);
    }

    [HttpGet("roles/{id}/permissions")]
    public async Task<IActionResult> GetRolePermissions(string id)
    {
        var permissions = await roleService.GetPermissionsByRoleIdAsync(id);

        return Ok(permissions);
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var role = await roleService.CreateRoleAsync(mapper.Map<CreateRoleDto>(request));

        return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
    }

    [HttpPut("roles")]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest request)
    {
        await roleService.UpdateRoleAsync(mapper.Map<UpdateRoleDto>(request));
        return NoContent();
    }
}
