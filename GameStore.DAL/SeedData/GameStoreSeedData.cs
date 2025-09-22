using Bogus;
using EFCore.BulkExtensions;
using GameStore.DAL.Data;
using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.SeedData;
public static class GameStoreSeedData
{
    // Platforms
    public static readonly Guid MobileId = Guid.Parse("b9df5c59-535b-4e72-80ba-92cf8f8f3b1a");
    public static readonly Guid BrowserId = Guid.Parse("dcfd6e47-bc5b-4f23-b8bc-b9f6e819243a");
    public static readonly Guid DesktopId = Guid.Parse("92492b13-fd1e-4747-8c87-df0833f028c7");
    public static readonly Guid ConsoleId = Guid.Parse("a9f59d2f-7d4f-4828-9a98-7b2a44730c68");

    // Genres
    public static readonly Guid StrategyId = Guid.Parse("6d6e4d0d-f979-4a91-86c6-435aaeef8383");
    public static readonly Guid SportsId = Guid.Parse("b4b3053b-df11-4e19-b14e-2f6842d123c3");
    public static readonly Guid RacesId = Guid.Parse("6cbe8193-94f5-4f6e-97f5-3b6353b8b84e");
    public static readonly Guid ActionId = Guid.Parse("046fe45b-c1ae-4e3b-b7a5-afe360b0d799");

    public static readonly Guid RPGId = Guid.Parse("a607de33-659e-453a-9f8e-667d7ae9cf6f");
    public static readonly Guid RTSId = Guid.Parse("df4516a0-7a5f-4900-a93e-3a05a34cb2f6");
    public static readonly Guid TBSId = Guid.Parse("859f9c41-09c0-47d7-a3f2-b01baf63dc37");
    public static readonly Guid RallyId = Guid.Parse("d1564358-22b3-4733-930e-78b161edcbe3");
    public static readonly Guid ArcadeId = Guid.Parse("d16c89b2-67ae-4915-8e93-3b686ec47c52");
    public static readonly Guid FormulaId = Guid.Parse("13b9b32d-f33d-4505-a144-d581b67239c4");
    public static readonly Guid OffRoadId = Guid.Parse("2cbe4021-14f6-4906-9457-26f14570fb0f");
    public static readonly Guid FPSId = Guid.Parse("bd01f92d-b682-4205-a62e-1f08e0eb28c9");
    public static readonly Guid TPSId = Guid.Parse("d77c1c69-9d88-41d4-a6f2-26a6a7a11cf7");
    public static readonly Guid AdventureId = Guid.Parse("25b3b6a7-909b-4e48-9340-6ae962c79c6b");
    public static readonly Guid PuzzleSkillId = Guid.Parse("11d5e40f-72b2-4d11-bef0-abb1a3a72be7");

    // Games
    public static readonly Guid Game1Id = Guid.Parse("2700f701-2822-4c19-81e5-33725969848e");
    public static readonly Guid Game2Id = Guid.Parse("2700f701-2822-4c19-81e5-33725969848d");
    public static readonly Guid Game3Id = Guid.Parse("2700f701-2822-4c19-81e5-33725969848f");

    // Publishers
    public static readonly Guid Publisher1Id = Guid.Parse("921b2618-b56a-4e93-9b85-483b7588bd4e");
    public static readonly Guid Publisher2Id = Guid.Parse("921b2618-b56a-4e93-9b85-483b7588bd4d");

    // Orders
    public static readonly Guid Order1Id = Guid.Parse("6c2e86e5-1584-40da-ba4d-43d44d1d84f3");
    public static readonly Guid Order2Id = Guid.Parse("6c2e86e5-1584-40da-ba4d-43d44d1d84f4");

    // OrderGames
    public static readonly Guid OrderGame1Id = Guid.Parse("6c2e86e5-1584-40da-ba4d-43d44d1d84a3");
    public static readonly Guid OrderGame2Id = Guid.Parse("6c2e86e5-1584-40da-ba4d-43d44d1d84a4");

    // Comments
    public static readonly Guid Comment1Id = Guid.Parse("6c2e86e5-1584-40da-ba4d-43d44d1d84d3");
    public static readonly Guid Comment2Id = Guid.Parse("6c2e86e5-1584-40da-ba4d-43d44d1d84d4");

    private static readonly List<Guid> _publisherIds =
    [
        Publisher1Id, Publisher2Id,
    ];

    private static readonly List<Guid> _platformIds =
    [
        MobileId, BrowserId, DesktopId, ConsoleId,
    ];

    private static readonly List<Guid> _genreIds =
    [
        StrategyId, SportsId, RacesId, ActionId, RPGId, RTSId, TBSId,
        RallyId, ArcadeId, FormulaId, OffRoadId, FPSId, TPSId,
        AdventureId, PuzzleSkillId,
    ];

    public static List<Game> GenerateFakeGames(int count)
    {
        var gameFaker = new Faker<Game>()
            .RuleFor(g => g.Id, f => Guid.NewGuid())
            .RuleFor(g => g.Name, f => "Fake " + f.Commerce.ProductName())
            .RuleFor(g => g.Key, f => f.Random.AlphaNumeric(10))
            .RuleFor(g => g.Description, f => f.Lorem.Paragraph())
            .RuleFor(g => g.Price, f => Math.Round(f.Random.Double(5.0, 100.0), 2))
            .RuleFor(g => g.UnitInStock, f => f.Random.Int(0, 10000))
            .RuleFor(g => g.Discount, f => f.Random.Int(0, 50))
            .RuleFor(g => g.PublisherId, f => f.PickRandom(_publisherIds))
            .RuleFor(g => g.ViewCount, f => 0)
            .RuleFor(g => g.CreatedAt, f => f.Date.Past(2))
            .RuleFor(g => g.ProductID, f => null)
            .RuleFor(g => g.QuantityPerUnit, f => f.Commerce.ProductAdjective())
            .RuleFor(g => g.UnitsOnOrder, f => null)
            .RuleFor(g => g.ReorderLevel, f => null)
            .RuleFor(g => g.Discontinued, f => f.Random.Bool(0.1f))
            .RuleFor(g => g.IsDeleted, f => false)
            .RuleFor(g => g.ImageName, f => null)
            .RuleFor(g => g.GameGenres, (f, g) =>
            [
                new GameGenre { GameId = g.Id, GenreId = f.PickRandom(_genreIds) },
            ])
            .RuleFor(g => g.GamePlatforms, (f, g) =>
            [
                new GamePlatform { GameId = g.Id, PlatformId = f.PickRandom(_platformIds) },
            ])
            .RuleFor(g => g.OrderGames, f =>
            [])
            .RuleFor(g => g.Comments, f =>
            []);

        return gameFaker.Generate(count);
    }

    public static void Seed(GameStoreContext context, int fakeGamesCount)
    {
        // Ensure database exists
        context.Database.EnsureCreated();

        // Seed Platforms
        if (!context.Platforms.IgnoreQueryFilters().Any())
        {
            context.Platforms.AddRange(
                new Platform { Id = MobileId, Type = "Mobile" },
                new Platform { Id = BrowserId, Type = "Browser" },
                new Platform { Id = DesktopId, Type = "Desktop" },
                new Platform { Id = ConsoleId, Type = "Console" });
        }

        // Seed Genres
        if (!context.Genres.IgnoreQueryFilters().Any())
        {
            context.Genres.AddRange(
                new Genre { Id = StrategyId, Name = "Strategy", ParentGenreId = null },
                new Genre { Id = RPGId, Name = "RPG", ParentGenreId = null },
                new Genre { Id = SportsId, Name = "Sports", ParentGenreId = null },
                new Genre { Id = ActionId, Name = "Action", ParentGenreId = null },
                new Genre { Id = RTSId, Name = "RTS", ParentGenreId = StrategyId },
                new Genre { Id = TBSId, Name = "TBS", ParentGenreId = StrategyId },
                new Genre { Id = RacesId, Name = "Races", ParentGenreId = SportsId },
                new Genre { Id = RallyId, Name = "Rally", ParentGenreId = RacesId },
                new Genre { Id = ArcadeId, Name = "Arcade", ParentGenreId = RacesId },
                new Genre { Id = FormulaId, Name = "Formula", ParentGenreId = RacesId },
                new Genre { Id = OffRoadId, Name = "Off-road", ParentGenreId = RacesId },
                new Genre { Id = FPSId, Name = "FPS", ParentGenreId = ActionId },
                new Genre { Id = TPSId, Name = "TPS", ParentGenreId = ActionId },
                new Genre { Id = AdventureId, Name = "Adventure", ParentGenreId = ActionId },
                new Genre { Id = PuzzleSkillId, Name = "Puzzle & Skill", ParentGenreId = null });
        }

        if (!context.Publishers.IgnoreQueryFilters().Any())
        {
            context.Publishers.AddRange(
                new Publisher { Id = Publisher1Id, CompanyName = "Game Publisher 1", HomePage = "https://www.google.com.ua/", Description = "Video games publisher 1" },
                new Publisher { Id = Publisher2Id, CompanyName = "Game Publisher 2", HomePage = "https://www.google.com.ua/", Description = "Video games publisher 2" });
        }

        context.SaveChanges();

        // Seed Games
        if (!context.Games.IgnoreQueryFilters().Any())
        {
            context.Games.AddRange(
                new Game
                {
                    Id = Game1Id,
                    Name = "Epic Strategy",
                    Key = "epic-stra",
                    Description = "Mobile strategy",
                    Price = 10.1,
                    UnitInStock = 1000,
                    Discount = 5,
                    PublisherId = Publisher1Id,
                },
                new Game
                {
                    Id = Game2Id,
                    Name = "Super Racer",
                    Key = "super-race",
                    Description = "Console race",
                    Price = 15.1,
                    UnitInStock = 3000,
                    Discount = 10,
                    PublisherId = Publisher2Id,
                },
                new Game
                {
                    Id = Game3Id,
                    Name = "FiFa",
                    Key = "fifa",
                    Description = "Sport simulator",
                    Price = 12.99,
                    UnitInStock = 4000,
                    Discount = 20,
                    PublisherId = Publisher2Id,
                });
            context.SaveChanges();

            // Seed GameGenres for the games
            context.GameGenres.AddRange(
                new GameGenre { GameId = Game1Id, GenreId = StrategyId },
                new GameGenre { GameId = Game2Id, GenreId = RacesId },
                new GameGenre { GameId = Game3Id, GenreId = SportsId });
            context.SaveChanges();

            // Seed GamePlatforms for the games
            context.GamePlatforms.AddRange(
                new GamePlatform { GameId = Game1Id, PlatformId = MobileId },
                new GamePlatform { GameId = Game2Id, PlatformId = ConsoleId },
                new GamePlatform { GameId = Game3Id, PlatformId = DesktopId });
            context.SaveChanges();
        }

        // Seed Orders
        if (!context.Orders.Any())
        {
            context.Orders.AddRange(
                new Order
                {
                    Id = Order1Id,
                    CustomerId = StubConstants.StubCustomerId,
                    Status = OrderStatus.Paid,
                    Date = DateTime.UtcNow,
                },
                new Order
                {
                    Id = Order2Id,
                    CustomerId = StubConstants.StubCustomerId,
                    Status = OrderStatus.Cancelled,
                    Date = DateTime.UtcNow,
                });
            context.SaveChanges();

            context.OrderGames.AddRange(
                new OrderGame
                {
                    Id = OrderGame1Id,
                    OrderId = Order1Id,
                    ProductId = Game1Id,
                    Quantity = 2,
                    Price = 10.1,
                    Discount = 5,
                },
                new OrderGame
                {
                    Id = OrderGame2Id,
                    OrderId = Order2Id,
                    ProductId = Game2Id,
                    Quantity = 2,
                    Price = 15.1,
                    Discount = 10,
                });

            context.SaveChanges();
        }

        if (!context.Comments.IgnoreQueryFilters().Any())
        {
            context.Comments.AddRange(
                new Comment()
                {
                    Id = Comment1Id,
                    Name = "Name1",
                    Body = "Text of comment 1",
                    GameId = Game1Id,
                },
                new Comment()
                {
                    Id = Comment2Id,
                    Name = "Name2",
                    Body = "Text of comment 2",
                    GameId = Game1Id,
                });

            context.SaveChanges();
        }

        if (!context.GameTranslations.IgnoreQueryFilters().Any())
        {
            context.GameTranslations.AddRange(
                new GameTranslation
                {
                    GameId = Game1Id,
                    LanguageCode = "uk",
                    Name = "Епічна стратегія",
                    Description = "Мобільна стратегія",
                },
                new GameTranslation
                {
                    GameId = Game2Id,
                    LanguageCode = "uk",
                    Name = "Супер гонки",
                    Description = "Консольні гонки",
                },
                new GameTranslation
                {
                    GameId = Game3Id,
                    LanguageCode = "uk",
                    Name = "ФіФа",
                    Description = "Спортивний симулятор",
                },
                new GameTranslation
                {
                    GameId = Game1Id,
                    LanguageCode = "es",
                    Name = "Estrategia Épica",
                    Description = "Estrategia móvil",
                },
                new GameTranslation
                {
                    GameId = Game2Id,
                    LanguageCode = "es",
                    Name = "Súper Carreras",
                    Description = "Carreras de consola",
                },
                new GameTranslation
                {
                    GameId = Game3Id,
                    LanguageCode = "es",
                    Name = "FIFA",
                    Description = "Simulador deportivo",
                });
            context.SaveChanges();
        }

        if (!context.Games.AsNoTracking().IgnoreQueryFilters().Any(g => g.Name.StartsWith("Fake")))
        {
            var fakeGames = GenerateFakeGames(fakeGamesCount);

            if (fakeGamesCount < 100)
            {
                context.Games.AddRange(fakeGames);
                context.SaveChanges();
            }
            else
            {
                context.BulkInsert(fakeGames, new BulkConfig { BulkCopyTimeout = 300 });
                var gameGenres = fakeGames.SelectMany(g => g.GameGenres).ToList();
                context.BulkInsert(gameGenres, new BulkConfig { BulkCopyTimeout = 300 });
                var gamePlatforms = fakeGames.SelectMany(g => g.GamePlatforms).ToList();
                context.BulkInsert(gamePlatforms, new BulkConfig { BulkCopyTimeout = 300 });
            }
        }
    }
}