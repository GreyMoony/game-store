using GameStore.Domain.Entities;

namespace GameStore.Application.Services.NotificationServices.NotificationStrategies;
public class SmsNotificationSender : INotificationSender
{
    public Task SendAsync(ApplicationUser user, string message, Order order)
    {
        Console.WriteLine($"[FAKE SMS] To: {user.Id}, Message: {message}");
        return Task.CompletedTask;
    }
}
