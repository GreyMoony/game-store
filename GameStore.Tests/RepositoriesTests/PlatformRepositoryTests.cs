using GameStore.DAL.Data;
using GameStore.DAL.Repositories;
using GameStore.Tests.TestUtilities;

namespace GameStore.Tests.RepositoriesTests;
public class PlatformRepositoryTests
{
    private readonly GameStoreContext _context;
    private readonly PlatformRepository _repository;
    private readonly TestData _testData;

    public PlatformRepositoryTests()
    {
        _context = GameStoreContextFactory.CreateInMemoryContext();
        _testData = TestDataSeeder.SeedDatabase(_context);
        _repository = new PlatformRepository(_context);
    }

    ~PlatformRepositoryTests()
    {
        _context.Database.EnsureDeletedAsync();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByGameKeyAsync_ValidKey_ShouldReturnPlatformList()
    {
        // arrange
        var gameKey = _testData.Games.First(g => g.GamePlatforms != null).Key;
        var actualPlatforms = _testData.Games.Where(game => game.Key == gameKey)
            .SelectMany(g => g.GamePlatforms)
            .Select(gp => _testData.Platforms.Where(p => p.Id == gp.PlatformId));

        // act
        var result = await _repository.GetByGameKeyAsync(gameKey);

        // assert
        Assert.Equal(actualPlatforms.Count(), result.Count());
    }

    [Fact]
    public void IdExist_ValidId_ShouldReturnTrue()
    {
        // Arrange
        var validId = _testData.Platforms[0].Id;

        // Assert
        Assert.True(_repository.IdExist(validId));
    }

    [Fact]
    public void IdExist_InvalidId_ShouldReturnFalse()
    {
        // Assert
        Assert.False(_repository.IdExist(Guid.NewGuid()));
    }
}
