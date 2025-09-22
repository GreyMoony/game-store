using GameStore.Domain.Entities;

namespace GameStore.Tests.TestUtilities;
public class AuthTestData
{
    public List<ApplicationUser> Users { get; set; }

    public List<UserNotificationMethod> Methods { get; set; }
}
