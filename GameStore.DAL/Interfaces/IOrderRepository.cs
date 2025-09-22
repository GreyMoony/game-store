using GameStore.Domain.Entities;

namespace GameStore.DAL.Interfaces;
public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetCartAsync(Guid customerId);

    Task<IEnumerable<Order>> GetPaidAndCancelledOrdersAsync();

    Task<IEnumerable<OrderGame>> GetOrderDetailsAsync(Guid orderId);

    Task<OrderGame?> GetOrderDetailByIdAsync(Guid id);

    Task<IEnumerable<Order>> GetByDateTimeAsync(DateTime? start, DateTime? end);

    void UpdateOrderGame(OrderGame orderGame);

    void DeleteOrderGame(OrderGame orderGame);

    Task AddOrderGameAsync(OrderGame orderGame);
}
