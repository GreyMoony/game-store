using GameStore.Application.DTOs.UserDtos;
using GameStore.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Application.Services.UserServices;
public interface IUserService
{
    Task<string> Login(LoginDto loginDto);

    Task<bool> CheckAccess(CheckAccessDto check, bool includeDeleted, List<UserRole> userRoles);

    Task<IEnumerable<UserDto>> GetAllUsersAsync();

    Task<UserDto> GetUserByIdAsync(string id);

    Task<IdentityResult> DeleteUserByIdAsync(string id);

    Task<UserDto> AddUserAsync(CreateUserDto request);

    Task UpdateUserAsync(UpdateUserDto request);
}
