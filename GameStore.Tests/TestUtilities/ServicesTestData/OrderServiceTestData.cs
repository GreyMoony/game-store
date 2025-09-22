using GameStore.Domain.Entities;
using GameStore.Domain.Enums;

namespace GameStore.Tests.TestUtilities.ServicesTestData;
public static class OrderServiceTestData
{
    private static readonly Guid FixedGameId = Guid.NewGuid();

    public static Game GameFromCart => new()
    {
        Id = FixedGameId,
        Key = "gameKey",
        UnitInStock = 5,
        Price = 20.0,
        Discount = 10,
    };

    public static Order CartWithOneGame => new()
    {
        Id = Guid.NewGuid(),
        CustomerId = Guid.NewGuid(),
        Status = OrderStatus.Open,
        OrderGames =
        [
            new()
            {
                ProductId = FixedGameId,
                Quantity = 1,
                Price = GameFromCart.Price,
                Discount = GameFromCart.Discount,
            },
        ],
    };

    public static Order CartWithTwoGames => new()
    {
        Id = Guid.NewGuid(),
        CustomerId = Guid.NewGuid(),
        Status = OrderStatus.Open,
        OrderGames =
        [
            new()
            {
                ProductId = FixedGameId,
                Quantity = 1,
                Price = GameFromCart.Price,
                Discount = GameFromCart.Discount,
            },
            new()
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1,
                Price = GameFromCart.Price,
                Discount = GameFromCart.Discount,
            },
        ],
    };

    public static Order CartWithMoreThenInStockGames => new()
    {
        Id = Guid.NewGuid(),
        CustomerId = Guid.NewGuid(),
        Status = OrderStatus.Open,
        OrderGames =
        [
            new()
            {
                ProductId = FixedGameId,
                Quantity = 6,
                Price = GameFromCart.Price,
                Discount = GameFromCart.Discount,
            },
        ],
    };

    public static Order Order => new()
    {
        Id = Guid.NewGuid(),
        CustomerId = Guid.NewGuid(),
        Status = OrderStatus.Paid,
        OrderGames =
        [
            new()
            {
                ProductId = FixedGameId,
                Quantity = 1,
                Price = GameFromCart.Price,
                Discount = GameFromCart.Discount,
            },
        ],
    };

    public static IEnumerable<Order> PaidAndCanceledOrders =>
        [
            new()
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Status = OrderStatus.Paid,
                Date = DateTime.Now,
            },
            new()
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Status = OrderStatus.Cancelled,
                Date = DateTime.Now,
            },
        ];
}
