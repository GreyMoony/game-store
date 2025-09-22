using FluentAssertions;
using GameStore.DAL.Data;
using GameStore.DAL.Repositories.MongoRepositories;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.MongoRepositoriesTestData;

namespace GameStore.Tests.RepositoriesTests;

[Collection("Mongo Collection")]
public class SupplierRepositoryIntegrationTests : IClassFixture<MongoTestFixture>
{
    private readonly SupplierRepository _repository;
    private readonly NorthwindDbContext _context;

    public SupplierRepositoryIntegrationTests(MongoTestFixture fixture)
    {
        _context = fixture.Context;
        _repository = new SupplierRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotDeleted_ShouldReturnSupplier()
    {
        // Arrange
        var id = MongoDbTestData.Suppliers[0].SupplierID;
        var companyName = MongoDbTestData.Suppliers[0].CompanyName;

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.CompanyName.Should().Be(companyName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenDeleted_ShouldReturnNull()
    {
        // Arrange
        var id = MongoDbTestData.Suppliers.Find(s => s.IsDeleted == true).SupplierID;

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByNameAsync_ShouldIgnoreDeletedSuppliers()
    {
        // Arrange
        var companyName = MongoDbTestData.Suppliers.Find(s => s.IsDeleted == true).CompanyName;

        // Act
        var result = await _repository.GetByNameAsync(companyName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByProductKeyAsync_WhenProductExists_ShouldReturnSupplier()
    {
        // Arrange
        var key = MongoDbTestData.Products[0].Key;
        var supplierId = MongoDbTestData.Products[0].SupplierID;

        // Act
        var result = await _repository.GetByProductKeyAsync(key);

        // Assert
        result.Should().NotBeNull();
        result!.SupplierID.Should().Be(supplierId);
    }

    [Fact]
    public async Task GetByProductKeyAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByProductKeyAsync("missing-key");

        // Assert
        result.Should().BeNull();
    }
}
