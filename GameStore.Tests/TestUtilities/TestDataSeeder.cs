using GameStore.DAL.Data;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;

namespace GameStore.Tests.TestUtilities;
public static class TestDataSeeder
{
    public static TestData SeedDatabase(GameStoreContext context)
    {
        var genre1 = new Genre { Id = Guid.NewGuid(), CategoryID = 1, Name = "Action" };
        var genre2 = new Genre { Id = Guid.NewGuid(), CategoryID = 2, Name = "RPG" };
        var genre3 = new Genre { Id = Guid.NewGuid(), CategoryID = 3, Name = "Race" };
        var genre4 = new Genre { Id = Guid.NewGuid(), CategoryID = 4, Name = "F1", ParentGenreId = genre3.Id };

        var platform1 = new Platform { Id = Guid.NewGuid(), Type = "PC" };
        var platform2 = new Platform { Id = Guid.NewGuid(), Type = "PlayStation" };
        var platform3 = new Platform { Id = Guid.NewGuid(), Type = "Mobile" };

        var publisher1 = new Publisher
        {
            Id = Guid.NewGuid(),
            CompanyName = "Publisher 1",
            Description = "Publisher 1 description",
            HomePage = "https://www.google.com.ua/",
        };

        var publisher2 = new Publisher
        {
            Id = Guid.NewGuid(),
            CompanyName = "Publisher 2",
            Description = "Publisher 2 description",
            HomePage = "https://www.google.com.ua/",
        };

        var game1Id = Guid.NewGuid();
        var game2Id = Guid.NewGuid();
        var game3Id = Guid.NewGuid();

        var game1 = new Game
        {
            Id = game1Id,
            ProductID = 1,
            Name = "Game 1",
            Key = "game-1",
            Description = "First game",
            Price = 30,
            UnitInStock = 3000,
            Discount = 10,
            GameGenres =
            [
                new GameGenre { GameId = game1Id, GenreId = genre1.Id },
            ],
            GamePlatforms =
            [
                new GamePlatform { GameId = game1Id, PlatformId = platform1.Id },
            ],
            PublisherId = publisher1.Id,
        };

        var game2 = new Game
        {
            Id = game2Id,
            ProductID = 2,
            Name = "Game 2",
            Key = "game-2",
            Description = "Second game",
            Price = 20,
            UnitInStock = 2000,
            Discount = 15,
            GameGenres =
            [
                new GameGenre { GameId = game2Id, GenreId = genre2.Id },
            ],
            GamePlatforms =
            [
                new GamePlatform { GameId = game2Id, PlatformId = platform2.Id },
            ],
            PublisherId = publisher1.Id,
        };

        var game3 = new Game
        {
            Id = game3Id,
            ProductID = 3,
            Name = "Game 3",
            Key = "game-3",
            Description = "Third game",
            Price = 10,
            UnitInStock = 3500,
            Discount = 5,
            GameGenres =
            [
                new GameGenre { GameId = game3Id, GenreId = genre1.Id },
            ],
            GamePlatforms =
            [
                new GamePlatform { GameId = game3Id, PlatformId = platform1.Id },
                new GamePlatform { GameId = game3Id, PlatformId = platform2.Id },
            ],
            PublisherId = publisher2.Id,
        };

        var order1Id = Guid.NewGuid();
        var order2Id = Guid.NewGuid();

        var customerId = Guid.NewGuid();

        var order1Games = new List<OrderGame>
        {
            new()
            {
                OrderId = order1Id,
                ProductId = game1Id,
                Price = game1.Price,
                Discount = game1.Discount,
                Quantity = 2,
            },
            new()
            {
                OrderId = order1Id,
                ProductId = game2Id,
                Price = game2.Price,
                Discount = game2.Discount,
                Quantity = 1,
            },
        };

        var order2Games = new List<OrderGame>
        {
            new()
            {
                OrderId = order2Id,
                ProductId = game1Id,
                Price = game1.Price,
                Discount = game1.Discount,
                Quantity = 3,
            },
        };

        var order1 = new Order
        {
            Id = order1Id,
            CustomerId = customerId,
            Date = DateTime.Now,
            Status = OrderStatus.Paid,
            OrderGames = order1Games,
        };

        var order2 = new Order
        {
            Id = order2Id,
            CustomerId = customerId,
            Date = DateTime.Now,
            Status = OrderStatus.Open,
            OrderGames = order2Games,
        };

        var comment1 = new Comment
        {
            Id = Guid.NewGuid(),
            Name = "Test user1 name",
            Body = "Test comment 1",
            GameId = game1Id,
        };

        var comment2 = new Comment
        {
            Id = Guid.NewGuid(),
            Name = "Test user2 name",
            Body = "Test comment 2",
            GameId = game1Id,
            ParentCommentId = comment1.Id,
        };

        context.Genres.AddRange(genre1, genre2, genre3, genre4);
        context.Platforms.AddRange(platform1, platform2, platform3);
        context.Publishers.AddRange(publisher1, publisher2);
        context.Games.AddRange(game1, game2, game3);
        context.Orders.AddRange(order1, order2);
        context.Comments.AddRange(comment1, comment2);

        context.SaveChanges();

        var games = new List<Game> { game1, game2, game3 };
        var genres = new List<Genre> { genre1, genre2, genre3, genre4 };
        var platforms = new List<Platform> { platform1, platform2, platform3 };
        var publishers = new List<Publisher> { publisher1, publisher2 };
        var orders = new List<Order> { order1, order2 };
        var comments = new List<Comment> { comment1, comment2 };

        return new TestData
        {
            Genres = genres,
            Platforms = platforms,
            Games = games,
            Publishers = publishers,
            Orders = orders,
            Comments = comments,
        };
    }

    public static AuthTestData SeedAuthDatabase(AuthDbContext context)
    {
        var users = new List<ApplicationUser>
        {
            new()
            {
                Id = "user1",
                UserName = "user1",
                Email = "user1@mail.com",
            },
            new()
            {
                Id = "user2",
                UserName = "user2",
                Email = "user2@mail.com",
            },
        };
        var notificationMethods = new List<UserNotificationMethod>
        {
            new()
            {
                UserId = "user1",
                Method = NotificationMethodType.Email,
            },
            new()
            {
                UserId = "user2",
                Method = NotificationMethodType.Push,
            },
        };

        context.Users.AddRange(users);
        context.UserNotificationMethods.AddRange(notificationMethods);
        context.SaveChanges();
        return new AuthTestData
        {
            Users = users,
            Methods = notificationMethods,
        };
    }
}