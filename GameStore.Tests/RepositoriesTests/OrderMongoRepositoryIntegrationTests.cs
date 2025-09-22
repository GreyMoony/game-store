using FluentAssertions;
using GameStore.DAL.Data;
using GameStore.DAL.Repositories.MongoRepositories;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.MongoRepositoriesTestData;

namespace GameStore.Tests.RepositoriesTests;

[Collection("Mongo Collection")]
public class OrderMongoRepositoryIntegrationTests : IClassFixture<MongoTestFixture>
{
    private readonly OrderMongoRepository _repository;
    private readonly NorthwindDbContext _context;

    public OrderMongoRepositoryIntegrationTests(MongoTestFixture fixture)
    {
        _context = fixture.Context;
        _repository = new OrderMongoRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnExistedOrder()
    {
        // Arrange
        var order = MongoDbTestData.Orders[0];
        var id = MongoDbTestData.Orders[0].OrderID;

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        Assert.Equal(order.OrderID, result!.OrderID);
    }

    [Fact]
    public async Task GetOrderDetailsAsync_ShouldReturnExistedOrderDetails()
    {
        // Arrange
        var orderDetails = MongoDbTestData.OrderDetails[0];
        var id = orderDetails.OrderID;

        // Act
        var result = await _repository.GetOrderDetailsAsync(id);

        // Assert
        // Assert that all order details have the same OrderID
        result.Should().NotBeNull();
        result.Should().AllSatisfy(od =>
        {
            od.OrderID.Should().Be(id);
        });
    }
}
