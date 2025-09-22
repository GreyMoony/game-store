using GameStore.Domain.Entities;

namespace GameStore.DAL.Interfaces;
public interface IPlatformRepository : IRepository<Platform>
{
    Task<IEnumerable<Platform>> GetByGameKeyAsync(string key, bool includeDeleted = false);

    bool IdExist(Guid id, bool includeDeleted = false);

    bool TypeExist(string type);
}
