using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories;

#pragma warning disable CA1862, CA1311
public class PublisherRepository(GameStoreContext context) : Repository<Publisher>(context), IPublisherRepository
{
    public async Task<Publisher?> GetByCompanyNameAsync(string companyName, bool includeDeleted = false)
    {
        var query = Context.Publishers.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query
            .FirstOrDefaultAsync(p => p.CompanyName.ToLower() == companyName.ToLower());
    }

    public async Task<Publisher?> GetByGameKeyAsync(string gameKey, bool includeDeleted = false)
    {
        var query = Context.Games.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query.Where(g => g.Key == gameKey)
            .Select(g => g.Publisher)
            .AsQueryable()
            .FirstOrDefaultAsync();
    }

    public async Task<Publisher?> GetBySupplierIdAsync(int id, bool includeDeleted = false)
    {
        var query = Context.Publishers.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query
            .FirstOrDefaultAsync(p => p.SupplierID == id);
    }

    public bool IdExist(Guid id, bool includeDeleted = false)
    {
        var query = Context.Publishers.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return query.Any(p => p.Id == id);
    }

    public bool NameExist(string companyName)
    {
        return Context.Publishers.IgnoreQueryFilters().AsEnumerable().Any(
            p => string.Equals(
                p.CompanyName,
                companyName,
                StringComparison.InvariantCultureIgnoreCase));
    }
}
