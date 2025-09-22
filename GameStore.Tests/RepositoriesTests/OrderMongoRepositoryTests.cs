using System.Globalization;
using GameStore.DAL.Data;
using GameStore.DAL.Repositories.MongoRepositories;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Settings;
using GameStore.Tests.TestUtilities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;

namespace GameStore.Tests.RepositoriesTests;
public class OrderMongoRepositoryTests
{
    private const string BrokenField = "field14";
    private readonly Mock<IMongoCollection<OrderNorthwind>> _orderCollectionMock;
    private readonly Mock<IMongoDatabase> _mongoDatabaseMock;
    private readonly Mock<IAsyncCursor<OrderNorthwind>> _asyncCursorMock;
    private readonly OrderMongoRepository _repository;

    public OrderMongoRepositoryTests()
    {
        _orderCollectionMock = new();
        _mongoDatabaseMock = new();
        _asyncCursorMock = new Mock<IAsyncCursor<OrderNorthwind>>();

        _mongoDatabaseMock.Setup(x => x.GetCollection<OrderNorthwind>(It.IsAny<string>(), null))
            .Returns(_orderCollectionMock.Object);

        var mongoDbSettings = new MongoDbSettings
        {
            OrderCollection = NorthwindNames.OrderCollection,
        };
        var optionsMock = new Mock<IOptions<MongoDbSettings>>();
        optionsMock.Setup(x => x.Value).Returns(mongoDbSettings);

        var dbContext = new NorthwindDbContext(_mongoDatabaseMock.Object, optionsMock.Object);
        _repository = new OrderMongoRepository(dbContext);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrders()
    {
        // Arrange
        var orders = new List<OrderNorthwind>
        {
            new() { OrderID = 1, OrderDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString() },
            new() { OrderID = 2, OrderDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc).ToString() },
        };

        MockSetupHelper.SetupCursorMock(_asyncCursorMock, orders, true);
        _orderCollectionMock.Setup(x => x.FindAsync(
            It.IsAny<FilterDefinition<OrderNorthwind>>(),
            It.IsAny<FindOptions<OrderNorthwind, OrderNorthwind>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(_asyncCursorMock.Object);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orders.Count, result.Count());
        Assert.Contains(result, o => o.OrderID == 1);
        Assert.Contains(result, o => o.OrderID == 2);
    }

    [Fact]
    public async Task GetByDateTimeAsync_ShouldReturnOrdersByDate()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc);
        var orders = new List<OrderNorthwind>
        {
            new()
            {
                OrderID = 1,
                OrderDate = startDate
                .ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
            },
            new()
            {
                OrderID = 2,
                OrderDate = endDate
                .ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
            },
            new()
            {
                OrderID = 2,
                OrderDate = new DateTime(2023, 2, 1, 0, 0, 0, DateTimeKind.Utc)
                .ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
            },
        };

        MockSetupHelper.SetupCursorMock(_asyncCursorMock, orders, true);
        var filter = Builders<OrderNorthwind>.Filter.Exists(BrokenField, false);
        _orderCollectionMock.Setup(x => x.FindAsync(
            It.Is<FilterDefinition<OrderNorthwind>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
            It.IsAny<FindOptions<OrderNorthwind>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(_asyncCursorMock.Object);

        // Act
        var result = await _repository.GetByDateTimeAsync(startDate, endDate);

        // Assert
        Assert.Equal(2, result.Count());
    }
}
