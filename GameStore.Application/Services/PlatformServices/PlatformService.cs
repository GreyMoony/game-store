using AutoMapper;
using GameStore.Application.DTOs.PlatformDtos;
using GameStore.Application.Helpers;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace GameStore.Application.Services.PlatformServices;
public class PlatformService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    EntityChangeLogger changeLogger) : IPlatformService
{
    public async Task<PlatformDto> AddPlatformAsync(AddPlatformDto platformDto)
    {
        ValidatePlatformDto(platformDto);

        var platform = mapper.Map<Platform>(platformDto);

        await unitOfWork.Platforms.AddAsync(platform);
        await unitOfWork.SaveAsync();

        changeLogger.LogEntityCreation(platform.Type, platform);
        return mapper.Map<PlatformDto>(platform);
    }

    public async Task DeletePlatformAsync(Guid id)
    {
        var platform = await unitOfWork.Platforms.GetByIdAsync(id)
            ?? throw new EntityNotFoundException($"Platform with Id {id} not found.");

        unitOfWork.Platforms.Delete(platform);
        changeLogger.LogEntityDeletion(platform.Type, platform);
        await unitOfWork.SaveAsync();
    }

    public async Task<IEnumerable<PlatformDto>> GetAllPlatformsAsync(bool includeDeleted = false)
    {
        var platforms = mapper.Map<IEnumerable<PlatformDto>>(
            await unitOfWork.Platforms.GetAllAsync(includeDeleted));

        return platforms;
    }

    public async Task<PlatformDto> GetPlatformByIdAsync(Guid id, bool includeDeleted = false)
    {
        var platform = await unitOfWork.Platforms.GetByIdAsync(id, includeDeleted)
            ?? throw new EntityNotFoundException($"Platform with Id {id} not found.");

        return mapper.Map<PlatformDto>(platform);
    }

    public async Task<IEnumerable<PlatformDto>> GetPlatformsByGameKeyAsync(string key, bool includeDeleted = false)
    {
        var platforms = await unitOfWork.Platforms.GetByGameKeyAsync(key, includeDeleted);
        return platforms.IsNullOrEmpty() ?
            []
            : mapper.Map<IEnumerable<PlatformDto>>(platforms);
    }

    public async Task UpdatePlatformAsync(PlatformDto platformDto, bool includeDeleted = false)
    {
        var platform = await unitOfWork.Platforms.GetByIdAsync(platformDto.Id, includeDeleted)
            ?? throw new EntityNotFoundException($"There is no platform with Id {platformDto.Id}");

        if (unitOfWork.Platforms.TypeExist(platformDto.Type) && platform.Type != platformDto.Type)
        {
            throw new UniquePropertyException($"Platform with type {platformDto.Type} already exist");
        }

        var oldPlatform = JsonConvert.DeserializeObject<Platform>(
            JsonConvert.SerializeObject(platform, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            }));

        platform.Type = platformDto.Type;

        unitOfWork.Platforms.Update(platform);
        await unitOfWork.SaveAsync();
        changeLogger.LogEntityChange("UPDATE", platform.Type, oldPlatform, platform);
    }

    private void ValidatePlatformDto(AddPlatformDto platformDto)
    {
        if (unitOfWork.Platforms.TypeExist(platformDto.Type))
        {
            throw new UniquePropertyException($"Platform with type {platformDto.Type} already exist");
        }
    }
}
