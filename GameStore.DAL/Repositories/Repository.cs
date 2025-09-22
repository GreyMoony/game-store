using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories;

#pragma warning disable S3604
public class Repository<T>(GameStoreContext context) : IRepository<T>
    where T : class
{
    protected GameStoreContext Context { get; } = context;

    public async Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false)
    {
        var query = Context.Set<T>().AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id, bool includeDeleted = false)
    {
        var query = Context.Set<T>().AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
    }

    public async Task AddAsync(T entity)
    {
        await Context.AddAsync(entity);
    }

    public void Update(T entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        var isDeletedProperty = typeof(T).GetProperty("IsDeleted");

        if (isDeletedProperty != null)
        {
            isDeletedProperty.SetValue(entity, true);
            Update(entity); // Mark entity as modified instead of removing it
        }
        else
        {
            Context.Set<T>().Remove(entity); // Hard delete only if soft deletion is not supported
        }
    }
}
