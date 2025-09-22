using GameStore.Application.DTOs.UserDtos;

namespace GameStore.Application.Services.UserServices;
public interface IBanService
{
    Task AddBannedUserAsync(BanDto banDto);

    Task<bool> IsUserBannedAsync(string name);
}
