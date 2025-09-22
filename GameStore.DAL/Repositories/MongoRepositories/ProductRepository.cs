using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameStore.DAL.Repositories.MongoRepositories;
public class ProductRepository(NorthwindDbContext context) : IProductRepository
{
    private const string MarkedCopiedField = NorthwindNames.MarkedCopiedField;

    public async Task<bool> DeleteByKeyAsync(string key)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Key, key);
        var update = Builders<Product>.Update.Set(s => s.IsDeleted, true);

        var result = await context.Products.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(bool includeDeleted = false)
    {
        var filter = Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Eq(MarkedCopiedField, false),
                Builders<Product>.Filter.Exists(MarkedCopiedField, false));

        if (!includeDeleted)
        {
            var notDeletedFilter = Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Eq(s => s.IsDeleted, false),
                Builders<Product>.Filter.Exists(NorthwindNames.IsDeletedField, false));
            filter = Builders<Product>.Filter.And(filter, notDeletedFilter);
        }

        return await (await context.Products.FindAsync(filter)).ToListAsync();
    }

    public IQueryable<Product> GetQuery(bool includeDeleted = false)
    {
        var query = context.Products.AsQueryable().Where(product =>
            !product.CopiedToSql.HasValue || product.CopiedToSql == false);

        if (!includeDeleted)
        {
            query = query.Where(product => !product.IsDeleted.HasValue || !product.IsDeleted.Value);
        }

        return query;
    }

    public async Task<Product?> GetByIdAsync(int id, bool includeDeleted = false)
    {
        var query = context.Products.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(product => !product.IsDeleted.HasValue || !product.IsDeleted.Value);
        }

        return await query.FirstOrDefaultAsync(p => p.ProductID == id);
    }

    public async Task<Product?> GetByKeyAsync(string key, bool includeDeleted = false)
    {
        var query = context.Products.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(product => !product.IsDeleted.HasValue || !product.IsDeleted.Value);
        }

        return await query.FirstOrDefaultAsync(p => p.Key == key);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, bool includeDeleted = false)
    {
        var query = context.Products.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(product => !product.IsDeleted.HasValue || !product.IsDeleted.Value);
        }

        return await query.Where(p => p.CategoryID == categoryId).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetBySupplierNameAsync(string name, bool includeDeleted = false)
    {
        var supplier = await context.Suppliers
        .AsQueryable()
        .FirstOrDefaultAsync(s => s.CompanyName == name);

        var query = context.Products.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(product => !product.IsDeleted.HasValue || !product.IsDeleted.Value);
        }

        return supplier == null ?
            []
            : await query.Where(p => p.SupplierID == supplier.SupplierID).ToListAsync();
    }

    public async Task<bool> MarkAsCopiedAsync(int id)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.ProductID, id);
        var update = Builders<Product>.Update.Set(p => p.CopiedToSql, true);

        var result = await context.Products.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> IncrementViewCount(string key)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Key, key);
        var update = Builders<Product>.Update.Inc(p => p.ViewCount, 1);

        var result = await context.Products.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }
}
