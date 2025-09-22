using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories;
public class GameRepository(GameStoreContext context) : Repository<Game>(context), IGameRepository
{
    public async Task<IEnumerable<Game>> GetByGenreAsync(Guid genreId, bool includeDeleted = false)
    {
        var query = Context.GameGenres.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query
            .Where(gg => gg.GenreId == genreId)
            .Select(gg => gg.Game)
            .AsNoTracking()
            .ToListAsync();
    }

    public new async Task<Game?> GetByIdAsync(Guid id, bool includeDeleted = false)
    {
        var query = Context.Games.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        query = query.Include(g => g.GameGenres)
            .Include(g => g.GamePlatforms).AsQueryable();

        return await query.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Game?> GetByProductIdAsync(int id, bool includeDeleted = false)
    {
        var query = Context.Games.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        query = query.Include(g => g.GameGenres)
                     .Include(g => g.GamePlatforms);

        return await query.FirstOrDefaultAsync(g => g.ProductID == id);
    }

    public async Task<Game?> GetByKeyAsync(string key, bool includeDeleted = false)
    {
        var query = Context.Games.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query.FirstOrDefaultAsync(g => g.Key == key);
    }

    public async Task<IEnumerable<Game>> GetByPlatformAsync(Guid platformId, bool includeDeleted = false)
    {
        var query = Context.GamePlatforms.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query
            .Where(gp => gp.PlatformId == platformId)
            .Select(gp => gp.Game)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetByPublisherAsync(string companyName, bool includeDeleted = false)
    {
        var query = Context.Games.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query
            .AsNoTracking()
            .Where(g => g.Publisher.CompanyName == companyName)
            .ToListAsync();
    }

    public IQueryable<Game> GetGamesQuery(bool includeDeleted = false)
    {
        var query = Context.Games.AsQueryable();

        var sql = query.ToQueryString();
        Console.WriteLine(sql);

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        query = query.AsNoTracking()
            .Include(g => g.GameGenres)
            .Include(g => g.GamePlatforms)
            .Include(g => g.Publisher)
            .Include(g => g.Comments);

        return query;
    }

    public Task<int> CountAsync(bool includeDeleted = false)
    {
        var query = Context.Games.AsNoTracking();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return query.CountAsync();
    }

    public async Task<IEnumerable<string>> GetAllKeysAsync()
    {
        return await Context.Games.IgnoreQueryFilters().Select(g => g.Key).ToListAsync();
    }

    public async Task<LocalizedGame?> GetLocalizedGameAsync(string key, string languageCode, bool includeDeleted = false)
    {
        var game = await Context.LocalizedGames.FromSqlRaw("SELECT * FROM dbo.GetGameLocalized({0}, {1}, {2})", key, languageCode, includeDeleted ? 1 : 0)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return game;
    }

    public async Task DeleteTranslationsAsync(Guid id)
    {
        var translations = await Context.GameTranslations
            .Where(gt => gt.GameId == id)
            .ToListAsync();

        if (translations.Count != 0)
        {
            Context.GameTranslations.RemoveRange(translations);
        }
    }
}
