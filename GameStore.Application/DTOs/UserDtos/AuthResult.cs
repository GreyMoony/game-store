namespace GameStore.Application.DTOs.UserDtos;
public class AuthResult
{
    public bool Success { get; set; }

    public AuthSuccessResponse? User { get; set; }

    public AuthErrorResponse? Error { get; set; }
}
