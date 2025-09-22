using GameStore.DAL.Data;
using GameStore.DAL.Repositories;
using GameStore.Tests.TestUtilities;

namespace GameStore.Tests.RepositoriesTests;
public class GenreRepositoryTests
{
    private readonly GameStoreContext _context;
    private readonly GenreRepository _repository;
    private readonly TestData _testData;

    public GenreRepositoryTests()
    {
        _context = GameStoreContextFactory.CreateInMemoryContext();
        _testData = TestDataSeeder.SeedDatabase(_context);
        _repository = new GenreRepository(_context);
    }

    ~GenreRepositoryTests()
    {
        _context.Database.EnsureDeletedAsync();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByGameKeyAsync_ValidGameKey_ShouldReturnGenreList()
    {
        // Arrange
        var gameKey = _testData.Games.First(g => g.GameGenres != null).Key;
        var actualGenres = _testData.Games.Where(game => game.Key == gameKey)
            .SelectMany(g => g.GameGenres)
            .Select(gg => _testData.Genres.Where(g => g.Id == gg.GenreId));

        // Act
        var result = await _repository.GetByGameKeyAsync(gameKey);

        // Assert
        Assert.Equal(actualGenres.Count(), result.Count());
    }

    [Fact]
    public async Task GetByParentId_ValidId_ShouldReturnGenreList()
    {
        // Arrange
        var parentId = _testData.Genres.First(g => g.ParentGenreId != null).ParentGenreId.GetValueOrDefault();
        var actualGenres = _testData.Genres.Where(g => g.ParentGenreId == parentId);

        // Act
        var result = await _repository.GetByParentIdAsync(parentId);

        // Assert
        Assert.Equal(actualGenres.Count(), result.Count());
    }

    [Fact]
    public async Task GetByParentId_InvalidId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetByParentIdAsync(Guid.NewGuid());

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void IdExist_ValidId_ShouldReturnTrue()
    {
        // Arrange
        var validId = _testData.Genres[0].Id;

        // Assert
        Assert.True(_repository.IdExist(validId));
    }

    [Fact]
    public void IdExist_InvalidId_ShouldReturnFalse()
    {
        // Assert
        Assert.False(_repository.IdExist(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnGenre()
    {
        // Arrange
        var validId = _testData.Genres[0].Id;

        // Act
        var result = await _repository.GetByIdAsync(validId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(validId, result.Id);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ValidId_ShouldReturnGenre()
    {
        // Arrange
        var validCategoryId = _testData.Genres[0].CategoryID;

        // Act
        var result = await _repository.GetByCategoryIdAsync(validCategoryId!.Value);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(validCategoryId, result.CategoryID);
    }

    [Fact]
    public void NameExist_ValidName_ShouldReturnTrue()
    {
        // Arrange
        var validName = _testData.Genres[0].Name;

        // Assert
        Assert.True(_repository.NameExist(validName));
    }
}
