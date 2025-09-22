using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories;
public class GenreRepository(GameStoreContext context) : Repository<Genre>(context), IGenreRepository
{
    public async Task<IEnumerable<Genre>> GetByGameKeyAsync(string key, bool includeDeleted = false)
    {
        var query = Context.Games
        .AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        query = query.Include(g => g.GameGenres)
                     .ThenInclude(gg => gg.Genre);

        var game = await query.FirstOrDefaultAsync(g => g.Key == key);

        return game?.GameGenres.Select(gg => gg.Genre) ??
            [];
    }

    public new async Task<Genre?> GetByIdAsync(Guid id, bool includeDeleted = false)
    {
        var query = Context.Genres.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        query = query.Include(g => g.SubGenres);

        return await query.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Genre?> GetByCategoryIdAsync(int id, bool includeDeleted = false)
    {
        var query = Context.Genres.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query.FirstOrDefaultAsync(g => g.CategoryID == id);
    }

    public async Task<IEnumerable<Genre>> GetByParentIdAsync(Guid parentId, bool includeDeleted = false)
    {
        var query = Context.Genres.Where(g => g.ParentGenreId == parentId);

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query.ToListAsync();
    }

    public bool IdExist(Guid id, bool includeDeleted = false)
    {
        var query = Context.Genres.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return query.Any(g => g.Id == id);
    }

    public bool NameExist(string name)
    {
        return Context.Genres.IgnoreQueryFilters().Any(g => g.Name == name);
    }
}
