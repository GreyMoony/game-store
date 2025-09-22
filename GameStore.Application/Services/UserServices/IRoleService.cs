using GameStore.Application.DTOs.UserDtos;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Application.Services.UserServices;
public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync();

    Task<RoleDto> GetByIdAsync(string id);

    Task<IdentityResult> DeleteRoleAsync(string id);

    Task<IEnumerable<RoleDto>> GetUsersRolesAsync(string id);

    Task<IEnumerable<string>> GetAllPermissionsAsync();

    Task<IEnumerable<string>> GetPermissionsByRoleIdAsync(string id);

    Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);

    Task UpdateRoleAsync(UpdateRoleDto updateRoleDto);
}
