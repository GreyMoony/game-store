using System.Security.Claims;
using GameStore.Application.DTOs.UserDtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Services.UserServices;

#pragma warning disable IDE0305 // Simplify collection initialization
public class RoleService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : IRoleService
{
    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        return await roleManager.Roles
            .Select(r => new RoleDto { Id = r.Id, Name = r.Name! })
            .ToListAsync();
    }

    public async Task<RoleDto> GetByIdAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id) ??
            throw new EntityNotFoundException($"Role with Id {id} not found");

        return new RoleDto { Id = role.Id, Name = role.Name! };
    }

    public async Task<IdentityResult> DeleteRoleAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id) ??
            throw new EntityNotFoundException($"Role with Id {id} not found");

        return await roleManager.DeleteAsync(role);
    }

    public async Task<IEnumerable<RoleDto>> GetUsersRolesAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id) ??
            throw new EntityNotFoundException($"User with id {id} not found");

        var roleNames = await userManager.GetRolesAsync(user);

        var roles = new List<RoleDto>();

        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                roles.Add(new RoleDto { Id = role.Id, Name = roleName });
            }
        }

        return roles;
    }

    public async Task<IEnumerable<string>> GetAllPermissionsAsync()
    {
        var roles = roleManager.Roles.ToList();
        var allPermissions = new HashSet<string>();

        foreach (var role in roles)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            var permissions = claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value);

            allPermissions.UnionWith(permissions);
        }

        return allPermissions.ToList();
    }

    public async Task<IEnumerable<string>> GetPermissionsByRoleIdAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id) ??
            throw new EntityNotFoundException($"Role with Id {id} not found");

        var claims = await roleManager.GetClaimsAsync(role);
        var permissions = claims
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .ToList();

        return permissions;
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        if (await roleManager.RoleExistsAsync(createRoleDto.Role.Name))
        {
            throw new UniquePropertyException(
                $"Role with name {createRoleDto.Role.Name} already exists.");
        }

        var role = new IdentityRole(createRoleDto.Role.Name);
        var result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            throw new EntityCreationException(
                "Failed to create role: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Add permissions (claims) to the role
        foreach (var permission in createRoleDto.Permissions)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
        }

        return new RoleDto { Id = role.Id, Name = role.Name! };
    }

    public async Task UpdateRoleAsync(UpdateRoleDto updateRoleDto)
    {
        var role = await roleManager.FindByIdAsync(updateRoleDto.Role.Id) ??
            throw new EntityNotFoundException($"Role with Id {updateRoleDto.Role.Id} not found");

        // Update role name
        if (updateRoleDto.Role.Name != role.Name)
        {
            var updateResult = await roleManager.SetRoleNameAsync(role, updateRoleDto.Role.Name);
            if (!updateResult.Succeeded)
            {
                throw new DbUpdateException("Failed to update role name.");
            }
        }

        // Update role permissions (claims)
        var existingClaims = await roleManager.GetClaimsAsync(role);
        foreach (var claim in existingClaims)
        {
            await roleManager.RemoveClaimAsync(role, claim);
        }

        // Add new permissions as claims
        foreach (var permission in updateRoleDto.Permissions)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
        }
    }
}
#pragma warning restore IDE0305 // Simplify collection initialization
