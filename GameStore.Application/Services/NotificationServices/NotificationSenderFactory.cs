using GameStore.Application.Services.NotificationServices.NotificationStrategies;
using GameStore.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Application.Services.NotificationServices;
public class NotificationSenderFactory(IServiceProvider serviceProvider) : INotificationSenderFactory
{
    public INotificationSender GetSender(NotificationMethodType method) =>
        method switch
        {
            NotificationMethodType.Email => serviceProvider.GetRequiredService<EmailNotificationSender>(),
            NotificationMethodType.Sms => serviceProvider.GetRequiredService<SmsNotificationSender>(),
            NotificationMethodType.Push => serviceProvider.GetRequiredService<PushNotificationSender>(),
            _ => throw new NotSupportedException($"Unknown method {method}"),
        };
}