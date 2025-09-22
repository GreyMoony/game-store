using GameStore.Application.DTOs.GameDtos;
using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices;

public interface IGameService
{
    Task<GameDto> AddGameAsync(AddGameDto game);

    Task<GameDto> GetGameByKeyAsync(string key, bool includeDeleted = false);

    Task<GameDto> GetGameByIdAsync(string id, bool includeDeleted = false);

    Task<IEnumerable<GameDto>> GetAllGamesAsync(bool includeDeleted = false);

    PagedGames GetGames(GameQuery query, bool includeDeleted = false);

    Task<IEnumerable<GameDto>> GetGamesByGenreAsync(string genreId, bool includeDeleted = false);

    Task<IEnumerable<GameDto>> GetGamesByPlatformAsync(Guid platformId, bool includeDeleted = false);

    Task<IEnumerable<GameDto>> GetGamesByPublisherAsync(string companyName, bool includeDeleted = false);

    Task<Game> UpdateGameAsync(UpdateGameDto game, bool includeDeleted = false);

    Task DeleteGameAsync(string key);

    Task<int> GamesCount(bool includeDeleted = false);

    Task IncrementViewCount(string gameKey);

    Task<GameImageDto?> GetGameImageAsync(string gameKey);

    Task<GameDto> GetLocalizedGameAsync(string key, string lang, bool includeDeleted = false);
}
