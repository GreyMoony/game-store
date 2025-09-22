namespace GameStore.Application.DTOs.NotificationDtos;

public class EmailNotificationItemDto
{
    public string GameTitle { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}