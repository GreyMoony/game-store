using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories;
public class PlatformRepository(GameStoreContext context) : Repository<Platform>(context), IPlatformRepository
{
    public async Task<IEnumerable<Platform>> GetByGameKeyAsync(string key, bool includeDeleted = false)
    {
        var query = Context.Games.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query
            .AsNoTracking()
            .Where(g => g.Key == key)
            .SelectMany(g => g.GamePlatforms.Select(gp => gp.Platform))
            .ToListAsync();
    }

    public bool IdExist(Guid id, bool includeDeleted = false)
    {
        var query = Context.Platforms.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return query.Any(p => p.Id == id);
    }

    public bool TypeExist(string type)
    {
        return Context.Platforms.IgnoreQueryFilters().Any(p => p.Type == type);
    }
}
