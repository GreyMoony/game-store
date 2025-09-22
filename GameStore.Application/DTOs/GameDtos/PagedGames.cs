namespace GameStore.Application.DTOs.GameDtos;
public class PagedGames
{
    public IEnumerable<GameDto> Games { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }
}
