using GameStore.Application.DTOs.GenreDtos;

namespace GameStore.Tests.TestUtilities.ControllersTestData;

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
public static class GenresControllerTestData
{
    public static List<ShortGenreDto> ListOfGenreDtos =>
        [
            new() { Name = "Test genre1" },
            new() { Name = "Test genre1" },
        ];
}
