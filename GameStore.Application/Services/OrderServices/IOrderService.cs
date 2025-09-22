using GameStore.Application.DTOs.OrderDtos;

namespace GameStore.Application.Services.OrderServices;
public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetPaidAndCancelledOrders();

    Task<OrderDto> GetOrderById(string id);

    Task<IEnumerable<OrderDetailsDto>> GetOrderDetails(string id);

    Task<Cart> GetCartAsync(Guid customerId);

    Task CheckoutOrder(Guid customerId);

    IEnumerable<PaymentMethodDto> GetPaymentMethods();

    Task AddGameToCartAsync(Guid customerId, string gameKey, int quantity = 1);

    Task RemoveGameFromCartAsync(Guid customerId, string gameKey);

    Task<PaymentResult> ProcessOrderPaymentAsync(Guid userId, PaymentRequestDto request);

    Task<IEnumerable<OrderDto>> GetOrderHistory(DateTime? start, DateTime? end);

    Task UpdateOrderDetailQuantityAsync(Guid id, int count);

    Task DeleteOrderDetailAsync(Guid id);

    Task ChangeStatusToShipped(Guid id);

    Task AddGameToOrder(Guid orderId, string gameKey);
}
