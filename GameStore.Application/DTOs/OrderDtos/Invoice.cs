namespace GameStore.Application.DTOs.OrderDtos;
public class Invoice
{
    public Guid UserId { get; set; }

    public Guid OrderId { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ValidUntil { get; set; }

    public double Sum { get; set; }
}
