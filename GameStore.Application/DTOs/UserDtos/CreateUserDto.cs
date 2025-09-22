namespace GameStore.Application.DTOs.UserDtos;
public class CreateUserDto
{
    public UserShortDto User { get; set; }

    public IEnumerable<string> Roles { get; set; }

    public string Password { get; set; }
}
