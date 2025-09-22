using GameStore.Domain.Entities.Mongo;

namespace GameStore.DAL.Interfaces;
public interface IOrderMongoRepository
{
    Task<IEnumerable<OrderNorthwind>> GetAllAsync();

    Task<IEnumerable<OrderNorthwind>> GetByDateTimeAsync(DateTime? start, DateTime? end);

    Task<OrderNorthwind?> GetByIdAsync(int id);

    Task<IEnumerable<OrderNorthwindDetail>> GetOrderDetailsAsync(int id);
}
