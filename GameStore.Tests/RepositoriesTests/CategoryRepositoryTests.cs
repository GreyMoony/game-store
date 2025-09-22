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
public class CategoryRepositoryTests
{
    private const string BrokenField = "field4";
    private const string MarkedCopiedField = NorthwindNames.MarkedCopiedField;

    private readonly Mock<IMongoCollection<Category>> _categoryCollectionMock;
    private readonly Mock<IMongoDatabase> _mongoDatabaseMock;
    private readonly Mock<IAsyncCursor<Category>> _asyncCursorMock;
    private readonly CategoryRepository _repository;

    public CategoryRepositoryTests()
    {
        _categoryCollectionMock = new();
        _mongoDatabaseMock = new();
        _asyncCursorMock = new Mock<IAsyncCursor<Category>>();

        _mongoDatabaseMock.Setup(x => x.GetCollection<Category>(It.IsAny<string>(), null))
            .Returns(_categoryCollectionMock.Object);

        var mongoDbSettings = new MongoDbSettings
        {
            CategoryCollection = "Categories",
            ProductCollection = "Products",
            SupplierCollection = "Suppliers",
            OrderCollection = "Orders",
            OrderDetailsCollection = "OrderDetails",
        };
        var optionsMock = new Mock<IOptions<MongoDbSettings>>();
        optionsMock.Setup(x => x.Value).Returns(mongoDbSettings);

        var dbContext = new NorthwindDbContext(_mongoDatabaseMock.Object, optionsMock.Object);
        _repository = new CategoryRepository(dbContext);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { CategoryID = 1, CategoryName = "Test1", IsDeleted = false },
            new() { CategoryID = 2, CategoryName = "Test2", IsDeleted = false },
        };

        var includeDeleted = false;
        var filter = Builders<Category>.Filter.And(
            Builders<Category>.Filter.Exists(BrokenField, false),
            Builders<Category>.Filter.Or(
                Builders<Category>.Filter.Eq(MarkedCopiedField, false),
                Builders<Category>.Filter.Exists(MarkedCopiedField, false)));
        var notDeletedFilter = Builders<Category>.Filter.Or(
                Builders<Category>.Filter.Eq(s => s.IsDeleted, false),
                Builders<Category>.Filter.Exists(NorthwindNames.IsDeletedField, false));
        filter = Builders<Category>.Filter.And(filter, notDeletedFilter);

        MockSetupHelper.SetupCursorMock(_asyncCursorMock, categories, true);

        _categoryCollectionMock.Setup(x => x.FindAsync(
            It.Is<FilterDefinition<Category>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
            It.IsAny<FindOptions<Category>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(_asyncCursorMock.Object);

        // Act
        var result = await _repository.GetAllAsync(includeDeleted);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task DeleteByIdAsync_ValidId_ShouldMarkAsDeleted()
    {
        // Arrange
        var id = 1;
        var filter = Builders<Category>.Filter.Eq(c => c.CategoryID, id);
        var update = Builders<Category>.Update.Set(c => c.IsDeleted, true);
        var updateResultMock = new Mock<UpdateResult>();
        updateResultMock.SetupGet(x => x.ModifiedCount).Returns(1);
        _categoryCollectionMock.Setup(x => x.UpdateOneAsync(
            It.Is<FilterDefinition<Category>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
            It.Is<UpdateDefinition<Category>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
            null,
            default)).ReturnsAsync(updateResultMock.Object);

        // Act
        var result = await _repository.DeleteByIdAsync(1);

        // Assert
        _categoryCollectionMock.Verify(
            x => x.UpdateOneAsync(
                It.Is<FilterDefinition<Category>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
                It.Is<UpdateDefinition<Category>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
                null,
                default),
            Times.Once);
        Assert.True(result);
    }

    [Fact]
    public async Task MarkAsCopiedAsync_ValidId_ShouldMarkAsCopied()
    {
        // Arrange
        var updateResultMock = new Mock<UpdateResult>();
        var id = 1;
        var filter = Builders<Category>.Filter.Eq(c => c.CategoryID, id);
        var update = Builders<Category>.Update.Set(c => c.CopiedToSql, true);
        updateResultMock.SetupGet(x => x.ModifiedCount).Returns(1);
        _categoryCollectionMock.Setup(x => x.UpdateOneAsync(
            It.Is<FilterDefinition<Category>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
            It.Is<UpdateDefinition<Category>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
            null,
            default)).ReturnsAsync(updateResultMock.Object);

        // Act
        var result = await _repository.MarkAsCopiedAsync(id);

        // Assert
        _categoryCollectionMock.Verify(
            x => x.UpdateOneAsync(
                It.Is<FilterDefinition<Category>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
                It.Is<UpdateDefinition<Category>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
                null,
                default),
            Times.Once);
        Assert.True(result);
    }

    [Fact]
    public async Task NameExist_CategoryExists_ReturnsTrue()
    {
        // Arrange
        var name = "Test";
        var filter = Builders<Category>.Filter.And(
            Builders<Category>.Filter.Exists(BrokenField, false),
            Builders<Category>.Filter.Or(
                Builders<Category>.Filter.Eq(MarkedCopiedField, false),
                Builders<Category>.Filter.Exists(MarkedCopiedField, false)),
            Builders<Category>.Filter.Eq("CategoryName", name));
        MockSetupHelper.SetupCursorMock(
            _asyncCursorMock,
            [new Category { CategoryName = "Test" }],
            true);
        _categoryCollectionMock.Setup(
            x => x.FindAsync(
                It.Is<FilterDefinition<Category>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
                It.IsAny<FindOptions<Category>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(_asyncCursorMock.Object);

        // Act
        var result = await _repository.NameExist(name);

        // Assert
        Assert.True(result);
    }
}
