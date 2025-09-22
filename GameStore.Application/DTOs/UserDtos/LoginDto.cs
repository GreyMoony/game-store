namespace GameStore.Application.DTOs.UserDtos;
public class LoginDto
{
    public string Login { get; set; }

    public string Password { get; set; }

    public bool InternalAuth { get; set; }
}
