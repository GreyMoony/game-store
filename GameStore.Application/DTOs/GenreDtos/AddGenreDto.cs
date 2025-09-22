namespace GameStore.Application.DTOs.GenreDtos;
public class AddGenreDto
{
    public string Name { get; set; }

    public string? ParentGenreId { get; set; }
}
