using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories;
public class NotificationsRepository(AuthDbContext context) : INotificationsRepository
{
    public async Task<List<string>> GetUserMethodsAsync(string userId)
    {
        return await context.UserNotificationMethods
            .Where(m => m.UserId == userId)
            .Select(m => m.Method.ToString().ToLowerInvariant())
            .ToListAsync();
    }

    public async Task UpdateUserMethodsAsync(string userId, IEnumerable<NotificationMethodType> methods)
    {
        var existing = await context.UserNotificationMethods
            .Where(m => m.UserId == userId)
            .ToListAsync();

        context.UserNotificationMethods.RemoveRange(existing);

        var newMethods = methods
            .Distinct()
            .Select(m => new UserNotificationMethod
            {
                UserId = userId,
                Method = m,
            });

        await context.UserNotificationMethods.AddRangeAsync(newMethods);
        await context.SaveChangesAsync();
    }
}
