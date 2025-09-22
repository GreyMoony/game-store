using GameStore.Domain.Entities;
using GameStore.Domain.Enums;

namespace GameStore.Application.Services.NotificationServices;
public interface INotificationService
{
    Task<IEnumerable<string>> GetUserNotificationMethodsAsync(string userId);

    Task UpdateUserNotificationMethodsAsync(string userId, IEnumerable<NotificationMethodType> methods);

    Task NotifyOrderStatusChangedAsync(Order order);
}
