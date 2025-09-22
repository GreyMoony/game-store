using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameStore.DAL.Repositories.MongoRepositories;
public class CategoryRepository(NorthwindDbContext context) : ICategoryRepository
{
    private const string BrokenField = "field4";
    private const string MarkedCopiedField = NorthwindNames.MarkedCopiedField;

    public async Task<Category?> GetByIdAsync(int id, bool includeDeleted = false)
    {
        var query = context.Categories.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(s => !s.IsDeleted.HasValue || !s.IsDeleted.Value);
        }

        return await query.FirstOrDefaultAsync(c => c.CategoryID == id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(bool includeDeleted = false)
    {
        var filter = Builders<Category>.Filter.And(
            Builders<Category>.Filter.Exists(BrokenField, false),
            Builders<Category>.Filter.Or(
                Builders<Category>.Filter.Eq(MarkedCopiedField, false),
                Builders<Category>.Filter.Exists(MarkedCopiedField, false)));

        if (!includeDeleted)
        {
            var notDeletedFilter = Builders<Category>.Filter.Or(
                Builders<Category>.Filter.Eq(s => s.IsDeleted, false),
                Builders<Category>.Filter.Exists(NorthwindNames.IsDeletedField, false));
            filter = Builders<Category>.Filter.And(filter, notDeletedFilter);
        }

        return await (await context.Categories.FindAsync(filter)).ToListAsync();
    }

    public async Task<Category?> GetByProductKeyAsync(string key, bool includeDeleted = false)
    {
        var product = await context.Products.AsQueryable().FirstOrDefaultAsync(p => p.Key == key);

        var query = context.Categories.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(s => !s.IsDeleted.HasValue || !s.IsDeleted.Value);
        }

        return product is null
            ? null
            : await query.FirstOrDefaultAsync(c => c.CategoryID == product.CategoryID);
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        var filter = Builders<Category>.Filter.Eq(c => c.CategoryID, id);
        var update = Builders<Category>.Update.Set(c => c.IsDeleted, true);

        var result = await context.Categories.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> MarkAsCopiedAsync(int id)
    {
        var filter = Builders<Category>.Filter.Eq(c => c.CategoryID, id);
        var update = Builders<Category>.Update.Set(c => c.CopiedToSql, true);

        var result = await context.Categories.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> NameExist(string name)
    {
        var filter = Builders<Category>.Filter.And(
            Builders<Category>.Filter.Exists(BrokenField, false),
            Builders<Category>.Filter.Or(
                Builders<Category>.Filter.Eq(MarkedCopiedField, false),
                Builders<Category>.Filter.Exists(MarkedCopiedField, false)),
            Builders<Category>.Filter.Eq("CategoryName", name));
        return await context.Categories.FindAsync(filter) is not null;
    }
}
