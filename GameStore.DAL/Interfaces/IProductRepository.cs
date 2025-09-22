using GameStore.Domain.Entities.Mongo;

namespace GameStore.DAL.Interfaces;
public interface IProductRepository
{
    Task<bool> DeleteByKeyAsync(string key);

    Task<IEnumerable<Product>> GetAllAsync(bool includeDeleted = false);

    IQueryable<Product> GetQuery(bool includeDeleted = false);

    Task<Product?> GetByIdAsync(int id, bool includeDeleted = false);

    Task<Product?> GetByKeyAsync(string key, bool includeDeleted = false);

    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, bool includeDeleted = false);

    Task<IEnumerable<Product>> GetBySupplierNameAsync(string name, bool includeDeleted = false);

    Task<bool> MarkAsCopiedAsync(int id);

    Task<bool> IncrementViewCount(string key);
}
