using GameStore.Domain.Entities;

namespace GameStore.DAL.Interfaces;
public interface IGameRepository : IRepository<Game>
{
    Task<Game?> GetByKeyAsync(string key, bool includeDeleted = false);

    Task<IEnumerable<Game>> GetByGenreAsync(Guid genreId, bool includeDeleted = false);

    Task<IEnumerable<Game>> GetByPlatformAsync(Guid platformId, bool includeDeleted = false);

    Task<IEnumerable<Game>> GetByPublisherAsync(string companyName, bool includeDeleted = false);

    Task<Game?> GetByProductIdAsync(int id, bool includeDeleted = false);

    IQueryable<Game> GetGamesQuery(bool includeDeleted = false);

    Task<int> CountAsync(bool includeDeleted = false);

    Task<IEnumerable<string>> GetAllKeysAsync();

    Task<LocalizedGame?> GetLocalizedGameAsync(string key, string languageCode, bool includeDeleted = false);

    Task DeleteTranslationsAsync(Guid id);
}
