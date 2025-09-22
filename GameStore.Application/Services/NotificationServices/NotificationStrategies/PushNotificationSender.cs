using GameStore.Domain.Entities;

namespace GameStore.Application.Services.NotificationServices.NotificationStrategies;
public class PushNotificationSender : INotificationSender
{
    public Task SendAsync(ApplicationUser user, string message, Order order)
    {
        Console.WriteLine($"[FAKE Push] To: {user.Id}, Message: {message}");
        return Task.CompletedTask;
    }
}
