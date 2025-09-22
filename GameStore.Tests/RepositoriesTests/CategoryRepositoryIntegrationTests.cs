using FluentAssertions;
using GameStore.DAL.Repositories.MongoRepositories;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.MongoRepositoriesTestData;

namespace GameStore.Tests.RepositoriesTests;

[Collection("Mongo Collection")]
public class CategoryRepositoryIntegrationTests(MongoTestFixture fixture) : IClassFixture<MongoTestFixture>
{
    private readonly CategoryRepository _repository = fixture.CategoryRepository;

    [Fact]
    public async Task GetByIdAsync_WhenNotDeleted_ShouldReturnCategory()
    {
        // Act
        var category = await _repository.GetByIdAsync(1);

        // Assert
        category.Should().NotBeNull();
        category!.CategoryName.Should().Be("Beverages");
    }

    [Fact]
    public async Task GetByProductKeyAsync_WhenNotDeleted_ShouldReturnCategory()
    {
        // Arrange
        var productKey = MongoDbTestData.Products[0].Key;

        // Act
        var category = await _repository.GetByProductKeyAsync(productKey);

        // Assert
        category.Should().NotBeNull();
        category!.CategoryName.Should().Be("Beverages");
    }
}
