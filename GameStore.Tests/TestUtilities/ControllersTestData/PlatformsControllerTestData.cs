using GameStore.Application.DTOs.PlatformDtos;

namespace GameStore.Tests.TestUtilities.ControllersTestData;

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
public static class PlatformsControllerTestData
{
    public static List<PlatformDto> ListOfPlatformDtos =>
        [
            new() { Type = "test platform1" },
            new() { Type = "test platform2" },
        ];
}
