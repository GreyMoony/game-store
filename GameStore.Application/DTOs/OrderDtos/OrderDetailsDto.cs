namespace GameStore.Application.DTOs.OrderDtos;
public class OrderDetailsDto
{
    public string Id { get; set; }

    public string ProductId { get; set; }

    public double Price { get; set; }

    public int Quantity { get; set; }

    public int Discount { get; set; }
}
