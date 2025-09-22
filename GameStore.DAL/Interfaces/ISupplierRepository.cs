using GameStore.Domain.Entities.Mongo;

namespace GameStore.DAL.Interfaces;
public interface ISupplierRepository
{
    Task<bool> DeleteByIdAsync(int id);

    Task<IEnumerable<Supplier>> GetAllAsync(bool includeDeleted = false);

    Task<Supplier?> GetByIdAsync(int id, bool includeDeleted = false);

    Task<Supplier?> GetByProductKeyAsync(string key, bool includeDeleted = false);

    Task<Supplier?> GetByNameAsync(string name, bool includeDeleted = false);

    Task<bool> NameExist(string name);

    Task<bool> MarkAsCopiedAsync(int id);
}
