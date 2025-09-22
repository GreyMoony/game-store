using GameStore.Application.DTOs.UserDtos;

namespace GameStore.Application.Services.UserServices;
public interface IExternalAuthService
{
    Task<AuthResult> AuthenticateAsync(LoginDto login);
}
