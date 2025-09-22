using GameStore.Application.DTOs.GenreDtos;

namespace GameStore.Application.Services.GenreServices;
public interface IGenreService
{
    Task<GenreDto> AddGenreAsync(AddGenreDto genreDto);

    Task<GenreDto> GetGenreByIdAsync(string id, bool includeDeleted = false);

    Task<IEnumerable<ShortGenreDto>> GetAllGenresAsync(bool includeDeleted = false);

    Task<IEnumerable<ShortGenreDto>> GetGenresByGameKeyAsync(string key, bool includeDeleted = false);

    Task<IEnumerable<ShortGenreDto>> GetGenresByParentIdAsync(string id, bool includeDeleted = false);

    Task UpdateGenreAsync(GenreDto genreDto, bool includeDeleted = false);

    Task DeleteGenreAsync(string id);
}
