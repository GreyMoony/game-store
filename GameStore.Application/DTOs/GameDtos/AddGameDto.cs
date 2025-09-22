namespace GameStore.Application.DTOs.GameDtos;
public class AddGameDto
{
    public ShortGameDto Game { get; set; }

    public IEnumerable<string> Genres { get; set; }

    public IEnumerable<Guid> Platforms { get; set; }

    public string Publisher { get; set; }

    public string Image { get; set; }

    public override string ToString()
    {
        return $"Game: {Game.Name}, Key: {Game.Key}, Description: {Game.Description}";
    }
}
