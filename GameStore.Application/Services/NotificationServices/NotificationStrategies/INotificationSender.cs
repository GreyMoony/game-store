using GameStore.Domain.Entities;

namespace GameStore.Application.Services.NotificationServices.NotificationStrategies;
public interface INotificationSender
{
    Task SendAsync(ApplicationUser user, string message, Order order);
}
