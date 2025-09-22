using GameStore.DAL.Data;
using GameStore.DAL.Repositories;
using GameStore.Tests.TestUtilities;

namespace GameStore.Tests.RepositoriesTests;
public class PublisherRepositoryTests
{
    private readonly GameStoreContext _context;
    private readonly PublisherRepository _repository;
    private readonly TestData _testData;

    public PublisherRepositoryTests()
    {
        _context = GameStoreContextFactory.CreateInMemoryContext();
        _testData = TestDataSeeder.SeedDatabase(_context);
        _repository = new PublisherRepository(_context);
    }

    ~PublisherRepositoryTests()
    {
        _context.Database.EnsureDeletedAsync();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByCompanyNameAsync_ValidCompanyName_ShouldReturnPublisher()
    {
        // Arrange
        var name = _testData.Publishers[0].CompanyName;
        var actualPublisher = _testData.Publishers[0];

        // Act
        var result = await _repository.GetByCompanyNameAsync(name);

        // Assert
        Assert.Equal(actualPublisher, result);
    }

    [Fact]
    public async Task GetByGameKeyAsync_ValidKey_ShouldReturnPublisher()
    {
        // Arrange
        var key = _testData.Games[0].Key;
        var actualPublisher = _testData.Publishers.Find(p => p.Id == _testData.Games[0].PublisherId);

        // Act
        var result = await _repository.GetByGameKeyAsync(key);

        // Assert
        Assert.Equal(actualPublisher, result);
    }

    [Fact]
    public void NameExist_ValidName_ShouldReturnTrue()
    {
        // Arrange
        var validName = _testData.Publishers[0].CompanyName;

        // Assert
        Assert.True(_repository.NameExist(validName));
    }

    [Fact]
    public void NameExist_InvalidName_ShouldReturnFalse()
    {
        // Assert
        Assert.False(_repository.NameExist("Not valid name"));
    }
}
