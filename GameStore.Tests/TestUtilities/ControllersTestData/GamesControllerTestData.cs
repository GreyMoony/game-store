using GameStore.Application.DTOs.GameDtos;

namespace GameStore.Tests.TestUtilities.ControllersTestData;

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
public static class GamesControllerTestData
{
    public static List<GameDto> ListOfGameDtos =>
        [
            new() { Name = "Test Game1", Key = "test-game1" },
            new() { Name = "Test Game1", Key = "test-game2" },
        ];

    public static GameDto GameDto => new()
    {
        Name = "Test Game",
        Key = "test-game",
        Description = "Returned game",
    };
}
