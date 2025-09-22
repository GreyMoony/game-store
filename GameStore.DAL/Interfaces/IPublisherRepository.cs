using GameStore.Domain.Entities;

namespace GameStore.DAL.Interfaces;
public interface IPublisherRepository : IRepository<Publisher>
{
    Task<Publisher?> GetByCompanyNameAsync(string companyName, bool includeDeleted = false);

    Task<Publisher?> GetByGameKeyAsync(string gameKey, bool includeDeleted = false);

    Task<Publisher?> GetBySupplierIdAsync(int id, bool includeDeleted = false);

    bool IdExist(Guid id, bool includeDeleted = false);

    bool NameExist(string companyName);
}
