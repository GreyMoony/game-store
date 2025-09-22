using GameStore.DAL.Data;
using GameStore.DAL.Repositories.MongoRepositories;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Settings;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.MongoRepositoriesTestData;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;

namespace GameStore.Tests.RepositoriesTests;
public class ProductRepositoryTests
{
    private const string MarkedCopiedField = NorthwindNames.MarkedCopiedField;

    private readonly Mock<IMongoCollection<Product>> _productCollectionMock;
    private readonly Mock<IMongoDatabase> _mongoDatabaseMock;
    private readonly Mock<IAsyncCursor<Product>> _asyncCursorMock;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        _productCollectionMock = new();
        _mongoDatabaseMock = new();
        _asyncCursorMock = new Mock<IAsyncCursor<Product>>();

        _mongoDatabaseMock.Setup(x => x.GetCollection<Product>(It.IsAny<string>(), null))
            .Returns(_productCollectionMock.Object);

        var mongoDbSettings = new MongoDbSettings
        {
            ProductCollection = NorthwindNames.ProductCollection,
        };
        var optionsMock = new Mock<IOptions<MongoDbSettings>>();
        optionsMock.Setup(x => x.Value).Returns(mongoDbSettings);

        var dbContext = new NorthwindDbContext(_mongoDatabaseMock.Object, optionsMock.Object);
        _repository = new ProductRepository(dbContext);
    }

    [Fact]
    public async Task DeleteByKeyAsync_ValidKey_ShouldMarkAsDeleted()
    {
        // Arrange
        var key = "key";
        var filter = Builders<Product>.Filter.Eq(p => p.Key, key);
        var update = Builders<Product>.Update.Set(s => s.IsDeleted, true);
        var updateResultMock = new Mock<UpdateResult>();
        updateResultMock.SetupGet(x => x.ModifiedCount).Returns(1);
        _productCollectionMock.Setup(x => x.UpdateOneAsync(
            It.Is<FilterDefinition<Product>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
            It.Is<UpdateDefinition<Product>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
            null,
            default)).ReturnsAsync(updateResultMock.Object);

        // Act
        var result = await _repository.DeleteByKeyAsync(key);

        // Assert
        _productCollectionMock.Verify(
            x => x.UpdateOneAsync(
                It.Is<FilterDefinition<Product>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
                It.Is<UpdateDefinition<Product>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
                null,
                default),
            Times.Once);
        Assert.True(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsProducts()
    {
        // Arrange
        var products = MongoDbTestData.Products;

        var includeDeleted = false;
        var filter = Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Eq(MarkedCopiedField, false),
                Builders<Product>.Filter.Exists(MarkedCopiedField, false));
        var notDeletedFilter = Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Eq(s => s.IsDeleted, false),
                Builders<Product>.Filter.Exists(NorthwindNames.IsDeletedField, false));
        filter = Builders<Product>.Filter.And(filter, notDeletedFilter);

        MockSetupHelper.SetupCursorMock(_asyncCursorMock, products, true);

        _productCollectionMock.Setup(x => x.FindAsync(
            It.Is<FilterDefinition<Product>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
            It.IsAny<FindOptions<Product>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(_asyncCursorMock.Object);

        // Act
        var result = await _repository.GetAllAsync(includeDeleted);

        // Assert
        Assert.Equal(products.Count, result.Count());
    }

    [Fact]
    public async Task MarkAsCopiedAsync_ValidId_ShouldMarkAsCopied()
    {
        // Arrange
        var updateResultMock = new Mock<UpdateResult>();
        var id = 1;
        var filter = Builders<Product>.Filter.Eq(p => p.ProductID, id);
        var update = Builders<Product>.Update.Set(p => p.CopiedToSql, true);
        updateResultMock.SetupGet(x => x.ModifiedCount).Returns(1);
        _productCollectionMock.Setup(x => x.UpdateOneAsync(
            It.Is<FilterDefinition<Product>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
            It.Is<UpdateDefinition<Product>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
            null,
            default)).ReturnsAsync(updateResultMock.Object);

        // Act
        var result = await _repository.MarkAsCopiedAsync(id);

        // Assert
        _productCollectionMock.Verify(
            x => x.UpdateOneAsync(
                It.Is<FilterDefinition<Product>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
                It.Is<UpdateDefinition<Product>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
                null,
                default),
            Times.Once);
        Assert.True(result);
    }

    [Fact]
    public async Task IncrementViewCount_ValidId_ShouldincrementViewCount()
    {
        // Arrange
        var updateResultMock = new Mock<UpdateResult>();
        var key = "productKey";
        var filter = Builders<Product>.Filter.Eq(p => p.Key, key);
        var update = Builders<Product>.Update.Inc(p => p.ViewCount, 1);
        updateResultMock.SetupGet(x => x.ModifiedCount).Returns(1);
        _productCollectionMock.Setup(x => x.UpdateOneAsync(
            It.Is<FilterDefinition<Product>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
            It.Is<UpdateDefinition<Product>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
            null,
            default)).ReturnsAsync(updateResultMock.Object);

        // Act
        var result = await _repository.IncrementViewCount(key);

        // Assert
        _productCollectionMock.Verify(
            x => x.UpdateOneAsync(
                It.Is<FilterDefinition<Product>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
                It.Is<UpdateDefinition<Product>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
                null,
                default),
            Times.Once);
        Assert.True(result);
    }
}
