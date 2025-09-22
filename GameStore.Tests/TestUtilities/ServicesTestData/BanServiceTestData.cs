using GameStore.Application.DTOs.UserDtos;
using GameStore.Application.Services.UserServices;
using GameStore.Domain.Entities;

namespace GameStore.Tests.TestUtilities.ServicesTestData;
public static class BanServiceTestData
{
    public static ApplicationUser BannedUser => new()
    {
        Id = Guid.NewGuid().ToString(),
        UserName = "BannedUser",
        IsBannedFromCommenting = true,
        BanEndTime = DateTime.UtcNow.AddDays(7),
    };

    public static ApplicationUser ExpiredBanUser => new()
    {
        Id = Guid.NewGuid().ToString(),
        UserName = "ExpiredBanUser",
        IsBannedFromCommenting = true,
        BanEndTime = DateTime.UtcNow.AddDays(-1),
    };

    public static ApplicationUser NotBannedUser => new()
    {
        Id = Guid.NewGuid().ToString(),
        UserName = "BannedUser",
        IsBannedFromCommenting = false,
        BanEndTime = null,
    };

    public static BanDto WeekBanDto => new()
    {
        User = "BannedUser",
        Duration = BanDurations.Week,
    };

    public static BanDto PermanentBanDto => new()
    {
        User = "BannedUser",
        Duration = BanDurations.Permanent,
    };
}
