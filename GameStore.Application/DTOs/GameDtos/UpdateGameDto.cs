namespace GameStore.Application.DTOs.GameDtos;
public class UpdateGameDto
{
    public GameDto Game { get; set; }

    public IEnumerable<string> Genres { get; set; }

    public IEnumerable<Guid> Platforms { get; set; }

    public string Publisher { get; set; }

    public string Image { get; set; }
}
