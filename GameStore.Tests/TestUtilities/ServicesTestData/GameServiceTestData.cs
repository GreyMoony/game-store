using System.Globalization;
using GameStore.Application.DTOs.GameDtos;
using GameStore.Application.Services.GameServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;

namespace GameStore.Tests.TestUtilities.ServicesTestData;

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
public static class GameServiceTestData
{
    public const string Genre1 = "11111111-1111-1111-1111-111111111111";
    public const string Genre2 = "22222222-2222-2222-2222-222222222222";
    public const string Platform1 = "33333333-3333-3333-3333-333333333333";
    public const string Platform2 = "44444444-4444-4444-4444-444444444444";
    public const string Publisher1 = "55555555-5555-5555-5555-555555555555";
    public const string Publisher2 = "66666666-6666-6666-6666-666666666666";

    public const string Category1 = "1";
    public const string Supplier1 = "2";

    public static Guid Genre1Guid => Guid.Parse(Genre1);

    public static Guid Genre2Guid => Guid.Parse(Genre2);

    public static Guid Platform1Guid => Guid.Parse(Platform1);

    public static Guid Platform2Guid => Guid.Parse(Platform2);

    public static Guid Publisher1Guid => Guid.Parse(Publisher1);

    public static Guid Publisher2Guid => Guid.Parse(Publisher2);

    public static AddGameDto InvalidKeyAddGameDto => new()
    {
        Game = new ShortGameDto { Name = "Test Game", Key = "test-key" },
        Genres = [],
        Platforms = [],
        Publisher = Publisher1,
    };

    public static AddGameDto NoKeyAddGameDto => new()
    {
        Game = new ShortGameDto { Name = "Test Game", Key = string.Empty },
        Genres = [],
        Platforms = [],
        Publisher = Publisher1,
    };

    public static AddGameDto AddGameDtoWithIds => new()
    {
        Game = new ShortGameDto { Name = "Test Game", Key = string.Empty },
        Genres = [Genre1],
        Platforms = [Guid.NewGuid()],
        Publisher = Publisher1,
    };

    public static UpdateGameDto UpdateGameDtoWithInvalidId => new()
    {
        Game = new GameDto { Id = "1-1-1", Name = "Updated Game", Key = "updated-key" },
        Genres = [Genre1],
        Platforms = [Guid.NewGuid()],
        Publisher = Publisher1,
    };

    public static UpdateGameDto UpdateGameDto => new()
    {
        Game = new GameDto { Id = "77777777-7777-7777-7777-777777777777", Name = "Updated Game", Key = "updated-key" },
        Genres = [Genre1],
        Platforms = [Guid.NewGuid()],
        Publisher = Publisher1,
    };

    public static UpdateGameDto UpdateProductGameDto => new()
    {
        Game = new GameDto { Id = "1", Name = "Updated Product", Key = "updated-key" },
        Genres = [Genre1, Category1],
        Platforms = [Guid.NewGuid()],
        Publisher = Supplier1,
    };

    public static Product OldProduct => new()
    {
        ProductID = 1,
        Name = "Old Product",
    };

    public static Category OldCategory => new()
    {
        CategoryID = int.Parse(Category1),
        CategoryName = "Category 1",
    };

    public static Supplier OldSupplier => new()
    {
        SupplierID = int.Parse(Supplier1),
        CompanyName = "Supplier 1",
    };

    public static Product OldCopiedProduct => new()
    {
        ProductID = 1,
        Name = "Old Product",
        CopiedToSql = true,
    };

    public static Game CopiedProduct => new()
    {
        ProductID = 1,
        Key = "old-key",
        Name = "Old Product",
        GameGenres = [],
        GamePlatforms = [],
    };

    public static Category OldCopiedCategory => new()
    {
        CategoryID = int.Parse(Category1),
        CategoryName = "Category 1",
        CopiedToSql = true,
    };

    public static Supplier OldCopiedSupplier => new()
    {
        SupplierID = int.Parse(Supplier1),
        CompanyName = "Supplier 1",
        CopiedToSql = true,
    };

    public static List<Game> GamesList =>
    [
            new() { Id = Guid.NewGuid(), Name = "Game 1", Key = "game-1" },
            new() { Id = Guid.NewGuid(), Name = "Game 2", Key = "game-2" },
    ];

    public static List<Product> ProductsList =>
    [
            new() { ProductID = 1, Name = "Product 1", Key = "product-1" },
            new() { ProductID = 2, Name = "Product 2", Key = "product-2" },
    ];

    public static Game GameEntity => new()
    {
        Id = Guid.NewGuid(),
        Name = "Game name",
        Key = "game-key",
        Description = "Game description",
    };

    public static Product ProductEntity => new()
    {
        ProductID = 1,
        Name = "Product name",
        Key = "product-key",
    };

    public static IEnumerable<object[]> GetGamesAsyncTestData()
    {
        yield return new object[]
        {
            new GameQuery { Page = 1, PageCount = "10", Sort = SortingOptions.MostPopular },
            new List<Game>
            {
                new() { Name = "Game 1", ViewCount = 100 },
                new() { Name = "Game 2", ViewCount = 200 },
            },
            new List<Product>
            {
                new() { Name = "Product 1", ViewCount = 300 },
            },
            new PagedGames
            {
                CurrentPage = 1,
                TotalPages = 1,
                Games =
                [
                    new GameDto { Name = "Product 1", Description = string.Empty },
                    new GameDto { Id = Guid.Empty.ToString(), Name = "Game 2" },
                    new GameDto { Id = Guid.Empty.ToString(), Name = "Game 1" }
                ],
            },
        };

        yield return new object[]
        {
            new GameQuery { Page = 1, PageCount = "10", Sort = SortingOptions.PriceDesc },
            new List<Game>
            {
                new() { Name = "Game 1", Price = 100 },
                new() { Name = "Game 2", Price = 200 },
            },
            new List<Product>
            {
                new() { Name = "Product 1", Price = 300 },
            },
            new PagedGames
            {
                CurrentPage = 1,
                TotalPages = 1,
                Games =
                [
                    new GameDto { Name = "Product 1", Price = 300, Description = string.Empty },
                    new GameDto { Id = Guid.Empty.ToString(), Name = "Game 2", Price = 200 },
                    new GameDto { Id = Guid.Empty.ToString(), Name = "Game 1", Price = 100 }
                ],
            },
        };

        yield return new object[]
        {
            new GameQuery { Page = 1, PageCount = "10", Sort = SortingOptions.Newest },
            new List<Game>
            {
                new() { Name = "Game 1", CreatedAt = DateTime.Parse("01-01-2025", CultureInfo.InvariantCulture) },
                new() { Name = "Game 2", CreatedAt = DateTime.Parse("02-01-2025", CultureInfo.InvariantCulture) },
            },
            new List<Product>
            {
                new() { Name = "Product 1", CreatedAt = DateTime.Parse("03-01-2025", CultureInfo.InvariantCulture) },
            },
            new PagedGames
            {
                CurrentPage = 1,
                TotalPages = 1,
                Games =
                [
                    new GameDto { Name = "Product 1", Description = string.Empty },
                    new GameDto { Id = Guid.Empty.ToString(), Name = "Game 2" },
                    new GameDto { Id = Guid.Empty.ToString(), Name = "Game 1" }
                ],
            },
        };

        yield return new object[]
        {
            new GameQuery
            {
                Page = 1,
                PageCount = "10",
                Sort = SortingOptions.PriceAsc,
                Genres =
                    [
                        Genre1,
                        Category1,
                    ],
                Platforms =
                    [
                        Platform1,
                    ],
                Publishers =
                    [
                        Publisher1,
                        Supplier1,
                    ],
                MaxPrice = 30,
                MinPrice = 5,
                Name = "Game",
            },
            new List<Game>
            {
                new()
                {
                    Name = "Game A",
                    Price = 10,
                    GameGenres =
                        [
                            new() { GenreId = Genre1Guid }
                        ],
                    GamePlatforms =
                        [
                            new() { PlatformId = Platform1Guid }
                        ],
                    PublisherId = Publisher1Guid,
                },
                new()
                {
                    Name = "Game B",
                    Price = 20,
                    GameGenres =
                        [
                            new() { GenreId = Genre1Guid }
                        ],
                    GamePlatforms =
                        [
                            new() { PlatformId = Platform1Guid }
                        ],
                    PublisherId = Publisher1Guid,
                },
                new()
                {
                    Name = "G!me C",
                    Price = 40,
                    GameGenres =
                        [
                            new() { GenreId = Genre2Guid }
                        ],
                    GamePlatforms =
                        [
                            new() { PlatformId = Platform2Guid }
                        ],
                    PublisherId = Publisher2Guid,
                },
            },
            new List<Product>
            {
                new()
                {
                    Name = "Product 1",
                    Price = 15,
                    CategoryID = int.Parse(Category1),
                    SupplierID = int.Parse(Supplier1),
                },
            },
            new PagedGames
            {
                CurrentPage = 1,
                TotalPages = 1,
                Games =
                [
                    new GameDto { Id = Guid.Empty.ToString(), Name = "Game A", Price = 10 },
                    new GameDto { Id = Guid.Empty.ToString(), Name = "Game B", Price = 20 },
                ],
            },
        };
    }
}
