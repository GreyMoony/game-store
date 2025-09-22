using GameStore.Application.DTOs.UserDtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Application.Services.UserServices;
public class BanUserIdentityService(UserManager<ApplicationUser> userManager) : IBanService
{
    public async Task AddBannedUserAsync(BanDto banDto)
    {
        TimeSpan? banDuration = BanDurations.GetBanDurationSpan(banDto.Duration);

        var user = await userManager.FindByNameAsync(banDto.User) ??
            throw new EntityNotFoundException($"User with name {banDto.User} is not found");

        if (user.IsBannedFromCommenting && user.BanEndTime is not null)
        {
            user.BanEndTime = banDuration is not null
                ? user.BanEndTime.Value.Add(banDuration.Value)
                : null;
            await userManager.UpdateAsync(user);
        }
        else if (!user.IsBannedFromCommenting)
        {
            user.IsBannedFromCommenting = true;
            user.BanEndTime = banDuration is null ? null : DateTime.UtcNow.Add(banDuration.Value);
            await userManager.UpdateAsync(user);
        }
    }

    public async Task<bool> IsUserBannedAsync(string name)
    {
        var user = await userManager.FindByNameAsync(name) ??
            throw new EntityNotFoundException($"User with name {name} is not found");

        if (user.IsBannedFromCommenting && user.BanEndTime < DateTime.UtcNow)
        {
            await UnbanUserAsync(user);
            return false;
        }
        else
        {
            return user.IsBannedFromCommenting && user.BanEndTime > DateTime.UtcNow;
        }
    }

    private async Task UnbanUserAsync(ApplicationUser user)
    {
        user.IsBannedFromCommenting = false;
        user.BanEndTime = null;
        await userManager.UpdateAsync(user);
    }
}
