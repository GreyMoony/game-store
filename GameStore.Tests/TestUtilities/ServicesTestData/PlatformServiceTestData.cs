using GameStore.Application.DTOs.PlatformDtos;
using GameStore.Domain.Entities;

namespace GameStore.Tests.TestUtilities.ServicesTestData;

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
public static class PlatformServiceTestData
{
    public static AddPlatformDto AddPlatformDto => new()
    {
        Type = "Test platform",
    };

    public static PlatformDto UpdatePlatformDto => new()
    {
        Id = Guid.NewGuid(),
        Type = "NewType",
    };

    public static List<Platform> PlatformList =>
    [
        new() { Type = "Test platform" },
        new() { Type = "Test platform 2" }
    ];

    public static Platform PlatformEntity => new()
    {
        Id = Guid.NewGuid(),
        Type = "Platform type",
    };
}
