using GameStore.DAL.Data;
using GameStore.DAL.Repositories;
using GameStore.Domain.Entities;
using GameStore.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Tests.RepositoriesTests;
public class GameRepositoryTests
{
    private readonly GameStoreContext _context;
    private readonly GameRepository _repository;
    private readonly TestData _testData;

    public GameRepositoryTests()
    {
        _context = GameStoreContextFactory.CreateInMemoryContext();
        _testData = TestDataSeeder.SeedDatabase(_context);
        _repository = new GameRepository(_context);
    }

    ~GameRepositoryTests()
    {
        _context.Database.EnsureDeletedAsync();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByKeyAsync_GameExists_ShouldReturnGame()
    {
        // Arrange
        var gameKey = _testData.Games[0].Key;

        // Act
        var result = await _repository.GetByKeyAsync(gameKey);

        // Assert
        Assert.Equal(_testData.Games[0], result);
    }

    [Fact]
    public async Task GetByKeyAsync_GameDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByKeyAsync("non-existing-key");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_GameExists_ShouldReturnGame()
    {
        // Arrange
        var gameId = _testData.Games[0].Id;

        // Act
        var result = await _repository.GetByIdAsync(gameId);

        // Assert
        Assert.Equal(_testData.Games[0], result);
    }

    [Fact]
    public async Task GetByIdAsync_GameDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByPlatformAsync_ValidPlatformId_ShouldReturnGameList()
    {
        // Arrange
        var platformId = _testData.Platforms[0].Id;
        var actualGames = _testData.Games.Where(g => g.GamePlatforms.Any(gp => gp.PlatformId == platformId));

        // Act
        var result = await _repository.GetByPlatformAsync(platformId);

        // Assert
        Assert.Equal(actualGames.Count(), result.Count());
    }

    [Fact]
    public async Task GetByPlatformAsync_PlatformWithoutGames_ShouldReturnEmptyList()
    {
        // Arrange
        var platform = _context.Platforms.FirstOrDefault(p => _context.GamePlatforms.All(gp => gp.PlatformId != p.Id));

        // Act
        var result = await _repository.GetByPlatformAsync(platform.Id);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByGenreAsync_ValidGenreId_ShouldReturnGameList()
    {
        // Arrange
        var genreId = _testData.Genres[0].Id;
        var actualGames = _testData.Games.Where(g => g.GameGenres.Any(gp => gp.GenreId == genreId));

        // Act
        var result = await _repository.GetByGenreAsync(genreId);

        // Assert
        Assert.Equal(actualGames.Count(), result.Count());
    }

    [Fact]
    public async Task GetByGenreAsync_GenreWithoutGames_ShouldReturnEmptyList()
    {
        // Arrange
        var genre = _context.Genres.FirstOrDefault(g => _context.GameGenres.All(gg => gg.GenreId != g.Id)); // Genre with no matching game

        // Act
        var result = await _repository.GetByGenreAsync(genre.Id);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnGameList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(_testData.Games.Count, result.Count());
    }

    [Fact]
    public async Task AddAsync_ValidGameEntity_ShouldAddNewGame()
    {
        // Arrange
        var game = new Game
        {
            Name = "New test game",
            Description = "Test game",
            Key = "new-test-game",
            IsDeleted = false,
            PublisherId = _testData.Publishers[0].Id,
        };

        // Act
        await _repository.AddAsync(game);
        await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(_testData.Games.Count + 1, _context.Games.Count());
        Assert.Contains(_context.Games, g => g.Name == game.Name);
    }

    [Fact]
    public async Task AddAsync_InvalidGameEntity_ThrowException()
    {
        // Arrange
        var game = new Game { };

        // Act & Assert
        await Assert.ThrowsAnyAsync<DbUpdateException>(async () =>
        {
            await _repository.AddAsync(game);
            _context.SaveChanges();
        });
    }

    [Fact]
    public async Task Update_ValidEntity_ShouldChangeEntityState()
    {
        // Arrange
        var game = _testData.Games[0];
        game.Description = "New description";

        // Act
        _repository.Update(game);
        await _context.SaveChangesAsync();

        // Assert
        var updatedGame = _context.Games.Find(game.Id);
        Assert.Equal(game.Description, updatedGame.Description);
    }

    [Fact]
    public async Task Update_InvalidEntity_ThrowException()
    {
        // Arrange
        var game = new Game { };

        // Act & Assert
        await Assert.ThrowsAnyAsync<DbUpdateException>(async () =>
        {
            _repository.Update(game);
            await _context.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task Delete_ExistingEntity_ShouldRemoveEntity()
    {
        // Arrange
        var game = _testData.Games[0];

        // Act
        _repository.Delete(game);
        await _context.SaveChangesAsync();

        // Assert
        var deletedGame = _context.Games.Find(game.Id);
        Assert.True(deletedGame.IsDeleted);
    }

    [Fact]
    public async Task Delete_NotExistingEntity_ThrowException()
    {
        // Arrange
        var game = new Game { };

        // Act & Assert
        await Assert.ThrowsAnyAsync<DbUpdateException>(async () =>
        {
            _repository.Delete(game);
            await _context.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task GetByProductIdAsync_ValidProductId_ShouldReturnGame()
    {
        // Arrange
        var productId = _testData.Games[0].ProductID;

        // Act
        var result = await _repository.GetByProductIdAsync(productId!.Value);

        // Assert
        Assert.Equal(_testData.Games[0], result);
    }

    [Fact]
    public async Task GetByPublisherAsync_ValidPublisherId_ShouldReturnGameList()
    {
        // Arrange
        var publisher = _testData.Publishers[0];
        var companyName = publisher.CompanyName;
        var actualGames = _testData.Games.Where(g => g.PublisherId == publisher.Id);

        // Act
        var result = await _repository.GetByPublisherAsync(companyName);

        // Assert
        Assert.Equal(actualGames.Count(), result.Count());
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        // Act
        var count = await _repository.CountAsync();

        // Assert
        Assert.Equal(_testData.Games.Count, count);
    }

    [Fact]
    public async Task GetAllKeysAsync_ShouldReturnAllGameKeys()
    {
        // Act
        var keys = await _repository.GetAllKeysAsync();

        // Assert
        Assert.Equal([.. _testData.Games.Select(g => g.Key)], keys);
    }
}
