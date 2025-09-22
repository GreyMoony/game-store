using GameStore.Domain.Entities;

namespace GameStore.DAL.Interfaces;
public interface IGenreRepository : IRepository<Genre>
{
    bool IdExist(Guid id, bool includeDeleted = false);

    bool NameExist(string name);

    Task<IEnumerable<Genre>> GetByGameKeyAsync(string key, bool includeDeleted = false);

    Task<IEnumerable<Genre>> GetByParentIdAsync(Guid parentId, bool includeDeleted = false);

    Task<Genre?> GetByCategoryIdAsync(int id, bool includeDeleted = false);
}
