using GameStore.Application.Services.NotificationServices.NotificationStrategies;
using GameStore.Domain.Enums;

namespace GameStore.Application.Services.NotificationServices;
public interface INotificationSenderFactory
{
    INotificationSender GetSender(NotificationMethodType method);
}
