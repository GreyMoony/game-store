using GameStore.DAL.Data;
using GameStore.DAL.Repositories.MongoRepositories;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.MongoRepositoriesTestData;

namespace GameStore.Tests.RepositoriesTests;

[Collection("Mongo Collection")]
public class ProductRepositoryIntegrationTests : IClassFixture<MongoTestFixture>
{
    private readonly ProductRepository _repository;
    private readonly NorthwindDbContext _context;

    public ProductRepositoryIntegrationTests(MongoTestFixture fixture)
    {
        _context = fixture.Context;
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public void GetQuery_ShouldReturnProductsNotCopiedToSql()
    {
        // Act
        var query = _repository.GetQuery();
        var result = query.ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, product => Assert.True(product.CopiedToSql is null or false));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnExistedProduct()
    {
        // Arrange
        var product = MongoDbTestData.Products[0];
        var id = product.ProductID;

        // Act
        var result = await _repository.GetByIdAsync(id!.Value);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.ProductID, result!.ProductID);
    }

    [Fact]
    public async Task GetByKeyAsync_ShouldReturnExistedProduct()
    {
        // Arrange
        var product = MongoDbTestData.Products[0];
        var key = product.Key;

        // Act
        var result = await _repository.GetByKeyAsync(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Key, result!.Key);
    }

    // Test for GetByCategoryAsync method
    [Fact]
    public async Task GetByCategoryAsync_ShouldReturnProductsInCategory()
    {
        // Arrange
        var category = MongoDbTestData.Categories[0];
        var categoryId = category.CategoryID;

        // Act
        var result = await _repository.GetByCategoryAsync(categoryId);

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, product => Assert.Equal(categoryId, product.CategoryID));
    }

    // Test for GetBySupplierNameAsync method
    [Fact]
    public async Task GetBySupplierNameAsync_ShouldReturnProductsBySupplierName()
    {
        // Arrange
        var supplier = MongoDbTestData.Suppliers[0];
        var supplierName = supplier.CompanyName;
        var supplierId = supplier.SupplierID;

        // Act
        var result = await _repository.GetBySupplierNameAsync(supplierName);

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, product => Assert.Equal(supplierId, product.SupplierID));
    }
}
