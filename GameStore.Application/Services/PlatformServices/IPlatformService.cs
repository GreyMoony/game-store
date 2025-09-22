using GameStore.Application.DTOs.PlatformDtos;

namespace GameStore.Application.Services.PlatformServices;
public interface IPlatformService
{
    Task<PlatformDto> AddPlatformAsync(AddPlatformDto platformDto);

    Task<PlatformDto> GetPlatformByIdAsync(Guid id, bool includeDeleted = false);

    Task<IEnumerable<PlatformDto>> GetAllPlatformsAsync(bool includeDeleted = false);

    Task<IEnumerable<PlatformDto>> GetPlatformsByGameKeyAsync(string key, bool includeDeleted = false);

    Task UpdatePlatformAsync(PlatformDto platformDto, bool includeDeleted = false);

    Task DeletePlatformAsync(Guid id);
}
