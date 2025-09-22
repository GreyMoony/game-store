namespace GameStore.DAL.Interfaces;
public interface IRepository<T>
    where T : class
{
    Task<T?> GetByIdAsync(Guid id, bool includeDeleted = false);

    Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false);

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);
}
