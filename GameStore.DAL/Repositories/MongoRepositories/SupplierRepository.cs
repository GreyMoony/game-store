using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameStore.DAL.Repositories.MongoRepositories;
#pragma warning disable CA1862, CA1311, IDE0046
public class SupplierRepository(NorthwindDbContext context) : ISupplierRepository
{
    private const string BrokenField = "field12";
    private const string MarkedCopiedField = NorthwindNames.MarkedCopiedField;

    public async Task<bool> DeleteByIdAsync(int id)
    {
        var filter = Builders<Supplier>.Filter.Eq(s => s.SupplierID, id);
        var update = Builders<Supplier>.Update.Set(s => s.IsDeleted, true);

        var result = await context.Suppliers.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync(bool includeDeleted = false)
    {
        var filter = Builders<Supplier>.Filter.And(
            Builders<Supplier>.Filter.Exists(BrokenField, false),
            Builders<Supplier>.Filter.Or(
                Builders<Supplier>.Filter.Eq(MarkedCopiedField, false),
                Builders<Supplier>.Filter.Exists(MarkedCopiedField, false)));

        if (!includeDeleted)
        {
            var notDeletedFilter = Builders<Supplier>.Filter.Or(
                Builders<Supplier>.Filter.Eq(s => s.IsDeleted, false),
                Builders<Supplier>.Filter.Exists(NorthwindNames.IsDeletedField, false));
            filter = Builders<Supplier>.Filter.And(filter, notDeletedFilter);
        }

        return await (await context.Suppliers.FindAsync(filter)).ToListAsync();
    }

    public async Task<Supplier?> GetByIdAsync(int id, bool includeDeleted = false)
    {
        var query = context.Suppliers.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(s => !s.IsDeleted.HasValue || !s.IsDeleted.Value);
        }

        return await query.FirstOrDefaultAsync(s => s.SupplierID == id);
    }

    public async Task<Supplier?> GetByProductKeyAsync(string key, bool includeDeleted = false)
    {
        var query = context.Products.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(p => !p.IsDeleted.HasValue || !p.IsDeleted.Value);
        }

        var product = await query.FirstOrDefaultAsync(p => p.Key == key);
        if (product is null)
        {
            return null;
        }
        else
        {
            return includeDeleted ?
                await context.Suppliers.AsQueryable()
                    .FirstOrDefaultAsync(s => s.SupplierID == product.SupplierID) :
                await context.Suppliers.AsQueryable().Where(s => !s.IsDeleted.HasValue || !s.IsDeleted.Value)
                    .FirstOrDefaultAsync(s => s.SupplierID == product.SupplierID);
        }
    }

    public async Task<Supplier?> GetByNameAsync(string name, bool includeDeleted = false)
    {
        var query = context.Suppliers.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(s => !s.IsDeleted.HasValue || !s.IsDeleted.Value);
        }

        return await query
            .FirstOrDefaultAsync(s => s.CompanyName.ToLower() == name.ToLower());
    }

    public async Task<bool> NameExist(string name)
    {
        var filter = Builders<Supplier>.Filter.And(
            Builders<Supplier>.Filter.Exists(BrokenField, false),
            Builders<Supplier>.Filter.Or(
                Builders<Supplier>.Filter.Eq(MarkedCopiedField, false),
                Builders<Supplier>.Filter.Exists(MarkedCopiedField, false)),
            Builders<Supplier>.Filter.Eq(NorthwindNames.CompanyNameField, name));
        return await context.Suppliers.FindAsync(filter) is not null;
    }

    public async Task<bool> MarkAsCopiedAsync(int id)
    {
        var filter = Builders<Supplier>.Filter.Eq(s => s.SupplierID, id);
        var update = Builders<Supplier>.Update.Set(s => s.CopiedToSql, true);

        var result = await context.Suppliers.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }
}
