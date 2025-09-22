using GameStore.Domain.Entities.Mongo;

namespace GameStore.DAL.Interfaces;
public interface ICategoryRepository
{
    Task<bool> DeleteByIdAsync(int id);

    Task<IEnumerable<Category>> GetAllAsync(bool includeDeleted = false);

    Task<Category?> GetByIdAsync(int id, bool includeDeleted = false);

    Task<Category?> GetByProductKeyAsync(string key, bool includeDeleted = false);

    Task<bool> MarkAsCopiedAsync(int id);

    Task<bool> NameExist(string name);
}
