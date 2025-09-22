using Microsoft.AspNetCore.Identity;

namespace GameStore.Domain.Entities;
public class ApplicationUser : IdentityUser
{
    public bool IsBannedFromCommenting { get; set; }

    public DateTime? BanEndTime { get; set; }

    public ICollection<UserNotificationMethod> NotificationMethods { get; set; }
}
