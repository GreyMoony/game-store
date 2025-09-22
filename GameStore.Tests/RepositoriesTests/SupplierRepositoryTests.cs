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
public class SupplierRepositoryTests
{
    private const string BrokenField = "field12";
    private const string MarkedCopiedField = NorthwindNames.MarkedCopiedField;

    private readonly Mock<IMongoCollection<Supplier>> _supplierCollectionMock;
    private readonly Mock<IMongoDatabase> _mongoDatabaseMock;
    private readonly Mock<IAsyncCursor<Supplier>> _asyncCursorMock;
    private readonly SupplierRepository _repository;

    public SupplierRepositoryTests()
    {
        _supplierCollectionMock = new();
        _mongoDatabaseMock = new();
        _asyncCursorMock = new Mock<IAsyncCursor<Supplier>>();

        _mongoDatabaseMock.Setup(x => x.GetCollection<Supplier>(It.IsAny<string>(), null))
            .Returns(_supplierCollectionMock.Object);

        var mongoDbSettings = new MongoDbSettings
        {
            SupplierCollection = "Suppliers",
        };
        var optionsMock = new Mock<IOptions<MongoDbSettings>>();
        optionsMock.Setup(x => x.Value).Returns(mongoDbSettings);

        var dbContext = new NorthwindDbContext(_mongoDatabaseMock.Object, optionsMock.Object);
        _repository = new SupplierRepository(dbContext);
    }

    [Fact]
    public async Task DeleteByIdAsync_ValidId_ShouldMarkAsDeleted()
    {
        // Arrange
        var id = 1;
        var filter = Builders<Supplier>.Filter.Eq(s => s.SupplierID, id);
        var update = Builders<Supplier>.Update.Set(s => s.IsDeleted, true);
        var updateResultMock = new Mock<UpdateResult>();
        updateResultMock.SetupGet(x => x.ModifiedCount).Returns(1);
        _supplierCollectionMock.Setup(x => x.UpdateOneAsync(
            It.Is<FilterDefinition<Supplier>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
            It.Is<UpdateDefinition<Supplier>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
            null,
            default)).ReturnsAsync(updateResultMock.Object);

        // Act
        var result = await _repository.DeleteByIdAsync(1);

        // Assert
        _supplierCollectionMock.Verify(
            x => x.UpdateOneAsync(
                It.Is<FilterDefinition<Supplier>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
                It.Is<UpdateDefinition<Supplier>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
                null,
                default),
            Times.Once);
        Assert.True(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSuppliers()
    {
        // Arrange
        var suppliers = MongoDbTestData.Suppliers
            .Where(s => !s.IsDeleted.HasValue || !s.IsDeleted.Value)
            .ToList();

        var includeDeleted = false;
        var filter = Builders<Supplier>.Filter.And(
            Builders<Supplier>.Filter.Exists(BrokenField, false),
            Builders<Supplier>.Filter.Or(
                Builders<Supplier>.Filter.Eq(MarkedCopiedField, false),
                Builders<Supplier>.Filter.Exists(MarkedCopiedField, false)));
        var notDeletedFilter = Builders<Supplier>.Filter.Or(
                Builders<Supplier>.Filter.Eq(s => s.IsDeleted, false),
                Builders<Supplier>.Filter.Exists(NorthwindNames.IsDeletedField, false));
        filter = Builders<Supplier>.Filter.And(filter, notDeletedFilter);

        MockSetupHelper.SetupCursorMock(_asyncCursorMock, suppliers, true);

        _supplierCollectionMock.Setup(x => x.FindAsync(
            It.Is<FilterDefinition<Supplier>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
            It.IsAny<FindOptions<Supplier>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(_asyncCursorMock.Object);

        // Act
        var result = await _repository.GetAllAsync(includeDeleted);

        // Assert
        Assert.Equal(suppliers.Count, result.Count());
    }

    [Fact]
    public async Task MarkAsCopiedAsync_ValidId_ShouldMarkAsCopied()
    {
        // Arrange
        var updateResultMock = new Mock<UpdateResult>();
        var id = 1;
        var filter = Builders<Supplier>.Filter.Eq(s => s.SupplierID, id);
        var update = Builders<Supplier>.Update.Set(s => s.CopiedToSql, true);
        updateResultMock.SetupGet(x => x.ModifiedCount).Returns(1);
        _supplierCollectionMock.Setup(x => x.UpdateOneAsync(
            It.Is<FilterDefinition<Supplier>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
            It.Is<UpdateDefinition<Supplier>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
            null,
            default)).ReturnsAsync(updateResultMock.Object);

        // Act
        var result = await _repository.MarkAsCopiedAsync(id);

        // Assert
        _supplierCollectionMock.Verify(
            x => x.UpdateOneAsync(
                It.Is<FilterDefinition<Supplier>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
                It.Is<UpdateDefinition<Supplier>>(u => u.RenderToBsonDocument().Equals(update.RenderToBsonDocument())),
                null,
                default),
            Times.Once);
        Assert.True(result);
    }

    [Fact]
    public async Task NameExist_SupplierExists_ReturnsTrue()
    {
        // Arrange
        var name = "Test";
        var filter = Builders<Supplier>.Filter.And(
            Builders<Supplier>.Filter.Exists(BrokenField, false),
            Builders<Supplier>.Filter.Or(
                Builders<Supplier>.Filter.Eq(MarkedCopiedField, false),
                Builders<Supplier>.Filter.Exists(MarkedCopiedField, false)),
            Builders<Supplier>.Filter.Eq(NorthwindNames.CompanyNameField, name));
        MockSetupHelper.SetupCursorMock(
            _asyncCursorMock,
            [new Supplier { CompanyName = "Test" }],
            true);
        _supplierCollectionMock.Setup(
            x => x.FindAsync(
                It.Is<FilterDefinition<Supplier>>(f => f.RenderToBsonDocument().Equals(filter.RenderToBsonDocument())),
                It.IsAny<FindOptions<Supplier>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(_asyncCursorMock.Object);

        // Act
        var result = await _repository.NameExist(name);

        // Assert
        Assert.True(result);
    }
}
