using GameStore.DAL.Interfaces;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;
using GameStore.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Application.Services.NotificationServices;
public class NotificationService(
    INotificationsRepository notificationsRepository,
    INotificationSenderFactory notificationSenderFactory,
    UserManager<ApplicationUser> userManager) : INotificationService
{
    public async Task<IEnumerable<string>> GetUserNotificationMethodsAsync(string userId)
    {
        var methods = await notificationsRepository.GetUserMethodsAsync(userId);

        return methods;
    }

    public async Task UpdateUserNotificationMethodsAsync(string userId, IEnumerable<NotificationMethodType> methods)
    {
        await notificationsRepository.UpdateUserMethodsAsync(userId, methods);
    }

    public async Task NotifyOrderStatusChangedAsync(Order order)
    {
        var recipients = await GetOrderNotifiedUsersAsync(order);

        string message = order.Status == OrderStatus.Checkout
            ? $"Order {order.Id} has been created successfully."
            : $"Order {order.Id} changed to status {order.Status}.";

        foreach (var user in recipients)
        {
            var methods = await GetUserNotificationMethodsAsync(user.Id);

            foreach (var methodString in methods)
            {
                if (!Enum.TryParse<NotificationMethodType>(methodString, true, out var method))
                {
                    continue;
                }

                var sender = notificationSenderFactory.GetSender(method);
                await sender.SendAsync(user, message, order);
            }
        }
    }

    private async Task<List<ApplicationUser>> GetOrderNotifiedUsersAsync(Order order)
    {
        var user = await userManager.FindByIdAsync(order.CustomerId.ToString())
            ?? throw new EntityNotFoundException($"User with id {order.CustomerId} not found");
        var recipients = new List<ApplicationUser>
        {
            user,
        };
        recipients.AddRange(await GetManagerUserIdsAsync());

        return recipients;
    }

    private async Task<List<ApplicationUser>> GetManagerUserIdsAsync()
    {
        var managers = (await userManager.GetUsersInRoleAsync(UserRoles.Manager)).ToList();

        return managers;
    }
}
