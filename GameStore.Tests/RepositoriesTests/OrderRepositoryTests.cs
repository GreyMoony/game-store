using GameStore.DAL.Data;
using GameStore.DAL.Repositories;
using GameStore.Domain.Enums;
using GameStore.Tests.TestUtilities;

namespace GameStore.Tests.RepositoriesTests;
public class OrderRepositoryTests
{
    private readonly GameStoreContext _context;
    private readonly OrderRepository _repository;
    private readonly TestData _testData;

    public OrderRepositoryTests()
    {
        _context = GameStoreContextFactory.CreateInMemoryContext();
        _testData = TestDataSeeder.SeedDatabase(_context);
        _repository = new OrderRepository(_context);
    }

    ~OrderRepositoryTests()
    {
        _context.Database.EnsureDeletedAsync();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnOrder()
    {
        // Arrange
        var id = _testData.Orders[0].Id;
        var actualOrder = _testData.Orders[0];

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        Assert.Equal(actualOrder, result);
    }

    [Fact]
    public async Task GetCartAsync_ValidCustomerId_ShouldReturnOpenOrder()
    {
        // Arrange
        var cart = _testData.Orders.Find(o => o.Status == OrderStatus.Open);
        var customerId = cart.CustomerId;

        // Act
        var result = await _repository.GetCartAsync(customerId);

        // Assert
        Assert.Equal(cart, result);
    }

    [Fact]
    public async Task GetOrderDetailsAsync_ValidOrderId_ShouldReturnOrderGamesList()
    {
        // Arrange
        var orderId = _testData.Orders[0].Id;
        var orderGames = _testData.Orders[0].OrderGames;

        // Act
        var result = await _repository.GetOrderDetailsAsync(orderId);

        // Assert
        Assert.Equal(orderGames, result);
    }

    [Fact]
    public async Task GetPaidAndCancelledOrdersAsync_ShouldReturnPaidAndCancelledOrdersList()
    {
        // Arrange
        var orders = _testData.Orders.Where(
            o => o.Status is OrderStatus.Paid
            or OrderStatus.Cancelled);

        // Act
        var result = await _repository.GetPaidAndCancelledOrdersAsync();

        // Assert
        Assert.Equal(orders, result);
    }
}
