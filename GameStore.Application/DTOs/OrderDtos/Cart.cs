namespace GameStore.Application.DTOs.OrderDtos;
public class Cart
{
    public Guid OrderId { get; set; }

    public IEnumerable<OrderDetailsDto> Details { get; set; }
}
