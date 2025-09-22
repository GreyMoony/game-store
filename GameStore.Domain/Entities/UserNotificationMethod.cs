using GameStore.Domain.Enums;

namespace GameStore.Domain.Entities;
public class UserNotificationMethod
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public NotificationMethodType Method { get; set; }

    public ApplicationUser User { get; set; }
}
