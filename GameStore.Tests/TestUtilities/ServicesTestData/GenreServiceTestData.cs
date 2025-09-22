using GameStore.Application.DTOs.GenreDtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;

namespace GameStore.Tests.TestUtilities.ServicesTestData;
public static class GenreServiceTestData
{
    public static AddGenreDto AddGenreDto => new()
    {
        Name = "Test addGenre name",
        ParentGenreId = "22222222-2222-2222-2222-222222222222",
    };

    public static AddGenreDto AddGenreDtoWithParentCategory => new()
    {
        Name = "Test addGenre name",
        ParentGenreId = "1",
    };

    public static GenreDto UpdateGenreDto => new()
    {
        Id = "11111111-1111-1111-1111-111111111111",
        Name = "Test updateGenre name",
        ParentGenreId = ValidParentGenreId,
    };

    public static GenreDto UpdateProductDto => new()
    {
        Id = "1",
        Name = "Test updateProduct name",
        ParentGenreId = ValidParentGenreId,
    };

    public static string ValidParentGenreId { get; set; } = "22222222-2222-2222-2222-222222222222";

    public static Genre GenreEntity => new()
    {
        Id = Guid.NewGuid(),
        Name = "Genre name",
    };

    public static Category Category => new()
    {
        CategoryID = 1,
        CategoryName = "Category name",
    };

    public static List<Genre> GenreList =>
        [
            new() { Id = Guid.NewGuid(), Name = "Genre 1" },
            new() { Id = Guid.NewGuid(), Name = "Genre 2" },
        ];
}
