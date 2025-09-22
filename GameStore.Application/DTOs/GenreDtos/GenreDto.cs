namespace GameStore.Application.DTOs.GenreDtos;
public class GenreDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string? ParentGenreId { get; set; }
}
