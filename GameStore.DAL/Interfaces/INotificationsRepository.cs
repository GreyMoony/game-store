using GameStore.Domain.Enums;

namespace GameStore.DAL.Interfaces;
public interface INotificationsRepository
{
    Task<List<string>> GetUserMethodsAsync(string userId);

    Task UpdateUserMethodsAsync(string userId, IEnumerable<NotificationMethodType> methods);
}
