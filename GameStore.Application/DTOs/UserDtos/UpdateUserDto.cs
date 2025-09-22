namespace GameStore.Application.DTOs.UserDtos;
public class UpdateUserDto
{
    public UserDto User { get; set; }

    public IEnumerable<string> Roles { get; set; }

    public string Password { get; set; }
}
