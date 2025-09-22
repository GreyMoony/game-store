namespace GameStore.Application.DTOs.NotificationDtos;
public class EmailNotificationDto
{
    public string UserId { get; set; }

    public string Email { get; set; }

    public string Message { get; set; }

    public string OrderId { get; set; }

    public string Status { get; set; }

    public List<EmailNotificationItemDto> Items { get; set; }

    public decimal TotalPrice { get; set; }
}
