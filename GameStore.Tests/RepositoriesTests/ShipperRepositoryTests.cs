using FluentAssertions;
using GameStore.DAL.Data;
using GameStore.DAL.Repositories.MongoRepositories;
using GameStore.Domain.Constants;
using GameStore.Domain.Settings;
using GameStore.Tests.TestUtilities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace GameStore.Tests.RepositoriesTests;

public class ShipperRepositoryTests
{
    private readonly Mock<IMongoCollection<BsonDocument>> _shipperCollectionMock;
    private readonly Mock<IMongoDatabase> _mongoDatabaseMock;
    private readonly ShipperRepository _repository;

    public ShipperRepositoryTests()
    {
        _shipperCollectionMock = new();
        _mongoDatabaseMock = new();

        _mongoDatabaseMock.Setup(x => x.GetCollection<BsonDocument>(It.IsAny<string>(), null))
            .Returns(_shipperCollectionMock.Object);

        var mongoDbSettings = new MongoDbSettings
        {
            ShipperCollection = NorthwindNames.ShipperCollection,
        };
        var optionsMock = new Mock<IOptions<MongoDbSettings>>();
        optionsMock.Setup(x => x.Value).Returns(mongoDbSettings);

        var dbContext = new NorthwindDbContext(_mongoDatabaseMock.Object, optionsMock.Object);
        _repository = new ShipperRepository(dbContext);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllShippersAsDictionary()
    {
        // Arrange: Insert sample shippers
        var shipperDocs = new[]
        {
            new BsonDocument { { "ShipperID", 1 }, { "CompanyName", "Shipper1" }, { "Phone", "123" } },
            new BsonDocument { { "ShipperID", 2 }, { "CompanyName", "Shipper2" }, { "Phone", "456" } },
        };

        var asyncCursorMock = new Mock<IAsyncCursor<BsonDocument>>();
        MockSetupHelper.SetupCursorMock(asyncCursorMock, shipperDocs, true);

        _shipperCollectionMock.Setup(x => x.FindAsync(
            It.IsAny<FilterDefinition<BsonDocument>>(),
            It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(asyncCursorMock.Object);

        // Act
        var result = await _repository.GetAll();

        // Assert
        result.Should().Contain(r => r["CompanyName"].ToString() == "Shipper1");
        result.Should().Contain(r => r["CompanyName"].ToString() == "Shipper2");
    }
}
