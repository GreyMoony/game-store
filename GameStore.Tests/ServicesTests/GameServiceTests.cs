using AutoMapper;
using FluentAssertions;
using GameStore.API.Mappings;
using GameStore.Application.DTOs.GameDtos;
using GameStore.Application.Helpers;
using GameStore.Application.Services.BlobStorageService;
using GameStore.Application.Services.GameServices;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Exceptions;
using GameStore.Tests.TestUtilities.ServicesTestData;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameStore.Tests.ServicesTests;

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
public class GameServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkMongo> _unitOfWorkMockMongo;
    private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMockMongo = new Mock<IUnitOfWorkMongo>();
        _blobStorageServiceMock = new Mock<IBlobStorageService>();
        _memoryCacheMock = new Mock<IMemoryCache>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        var loggerMock = new Mock<ILogger<GameService>>();
        var mockLogger = new Mock<ILogger<EntityChangeLogger>>();
        var changeLogger = new EntityChangeLogger(mockLogger.Object);

        _gameService = new GameService(
            _unitOfWorkMock.Object,
            _unitOfWorkMockMongo.Object,
            _blobStorageServiceMock.Object,
            mapper,
            _memoryCacheMock.Object,
            loggerMock.Object,
            changeLogger);
    }

    [Fact]
    public async Task AddGameAsync_ManualKeyProvidedIsInvalid_ThrowsKeyIsNotValidException()
    {
        // Arrange
        var gameDto = GameServiceTestData.InvalidKeyAddGameDto;

        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync("test-key", true))
            .ReturnsAsync(new Game() { Key = "test-key" }); // Simulate key already exists
        _unitOfWorkMock.Setup(u => u.Publishers.IdExist(Guid.Parse(gameDto.Publisher), false)).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<KeyIsNotValidException>(() => _gameService.AddGameAsync(gameDto));
    }

    [Fact]
    public async Task AddGameAsync_KeyIsEmpty_CreatesKey()
    {
        // Arrange
        var addGameDto = GameServiceTestData.NoKeyAddGameDto;

        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(It.IsAny<string>(), false))
            .ReturnsAsync((Game)null); // No game with this key
        _unitOfWorkMock.Setup(u => u.Publishers.IdExist(Guid.Parse(addGameDto.Publisher), false)).Returns(true);

        // Act
        var result = await _gameService.AddGameAsync(addGameDto);

        // Assert
        Assert.NotEmpty(result.Key);
    }

    [Fact]
    public async Task AddGameAsync_InvalidPublisherId_ThrowsIdsNotValidException()
    {
        // Arrange
        var addGameDto = GameServiceTestData.AddGameDtoWithIds;

        _unitOfWorkMock.Setup(u => u.Genres.IdExist(It.IsAny<Guid>(), false)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Platforms.IdExist(It.IsAny<Guid>(), false)).Returns(false);
        _unitOfWorkMock.Setup(
            u => u.Publishers.
            GetByIdAsync(Guid.Parse(addGameDto.Publisher), false))
            .ReturnsAsync((Publisher)null);

        // Act & Assert
        await Assert.ThrowsAsync<IdsNotValidException>(() => _gameService.AddGameAsync(addGameDto));
    }

    [Fact]
    public async Task AddGameAsync_InvalidGenreIds_ThrowsIdsNotValidException()
    {
        // Arrange
        var gameDto = GameServiceTestData.AddGameDtoWithIds;

        _unitOfWorkMock.Setup(u => u.Genres
        .IdExist(Guid.Parse(gameDto.Genres.First()), false)).Returns(false); // Simulate that the ID does not exist

        // Act & Assert
        await Assert.ThrowsAsync<IdsNotValidException>(() => _gameService.AddGameAsync(gameDto));
    }

    [Fact]
    public async Task AddGameAsync_InvalidPlatformIds_ThrowsIdsNotValidException()
    {
        // Arrange
        var gameDto = GameServiceTestData.AddGameDtoWithIds;

        _unitOfWorkMock.Setup(u => u.Genres.IdExist(Guid.Parse(gameDto.Genres.First()), false)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Platforms.IdExist(gameDto.Platforms.First(), false)).Returns(false); // Simulate that the ID does not exist

        // Act & Assert
        await Assert.ThrowsAsync<IdsNotValidException>(() => _gameService.AddGameAsync(gameDto));
    }

    [Fact]
    public async Task DeleteGameAsync_ValidKey_ShouldDeleteGame()
    {
        // Arrange
        var gameKey = "valid-key";
        var game = new Game { Id = Guid.NewGuid(), Key = gameKey };

        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(gameKey, false))
            .ReturnsAsync(game);

        // Act
        await _gameService.DeleteGameAsync(gameKey);

        // Assert
        _unitOfWorkMock.Verify(u => u.Games.Delete(game), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_InvalidKey_ThrowsEntityNotFoundException()
    {
        // Arrange
        var gameKey = "invalid-key";
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(gameKey, false))
            .ReturnsAsync((Game)null); // No game found
        _unitOfWorkMockMongo.Setup(u => u.Products.DeleteByKeyAsync(gameKey)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.DeleteGameAsync(gameKey));
    }

    [Fact]
    public async Task UpdateGameAsync_ValidData_ShouldUpdateGame()
    {
        // Arrange
        var updateGameDto = GameServiceTestData.UpdateGameDto;

        var existingGame = new Game
        {
            Id = Guid.Parse(updateGameDto.Game.Id),
            GameGenres = [],
            GamePlatforms = [],
        };

        _unitOfWorkMock.Setup(u => u.Genres.IdExist(Guid.Parse(updateGameDto.Genres.First()), true)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Platforms.IdExist(updateGameDto.Platforms.First(), true)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Games.GetByIdAsync(existingGame.Id, false))
            .ReturnsAsync(existingGame);
        _unitOfWorkMock.Setup(u => u.Publishers.IdExist(Guid.Parse(updateGameDto.Publisher), false)).Returns(true);

        // Act
        await _gameService.UpdateGameAsync(updateGameDto);

        // Assert
        _unitOfWorkMock.Verify(u => u.Games.Update(It.IsAny<Game>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateGameAsync_InvalidGameIdFomat_ThrowsEntityNotFoundException()
    {
        // Arrange
        var updateGameDto = GameServiceTestData.UpdateGameDtoWithInvalidId;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _gameService.UpdateGameAsync(updateGameDto));
    }

    [Fact]
    public async Task UpdateGameAsync_InvalidIds_ThrowsIdsNotValidException()
    {
        // Arrange
        var updateGameDto = GameServiceTestData.UpdateGameDto;
        var guidId = Guid.Parse(updateGameDto.Game.Id);

        _unitOfWorkMock.Setup(u => u.Games.GetByIdAsync(guidId, false))
            .ReturnsAsync(new Game { Id = guidId });
        _unitOfWorkMock.Setup(u => u.Genres.IdExist(Guid.Parse(updateGameDto.Genres.First()), false))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<IdsNotValidException>(() => _gameService.UpdateGameAsync(updateGameDto));
    }

    [Fact]
    public async Task UpdateGameAsync_InvalidKey_ThrowsKeyIsNotValidException()
    {
        // Arrange
        var updateGameDto = GameServiceTestData.UpdateGameDto;
        var guidId = Guid.Parse(updateGameDto.Game.Id);
        var gameWithKey = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Game with provided key",
            Key = updateGameDto.Game.Key,
        };
        var gameToUpdate = new Game
        {
            Id = guidId,
            Name = "Game to update",
            Key = "Another Key",
        };

        _unitOfWorkMock.Setup(u => u.Games.GetByIdAsync(guidId, false))
            .ReturnsAsync(gameToUpdate);
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(updateGameDto.Game.Key, true))
            .ReturnsAsync(gameWithKey);
        _unitOfWorkMock.Setup(u => u.Genres.IdExist(Guid.Parse(updateGameDto.Genres.First()), true)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Platforms.IdExist(updateGameDto.Platforms.First(), true)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Publishers.IdExist(Guid.Parse(updateGameDto.Publisher), false)).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<KeyIsNotValidException>(() => _gameService.UpdateGameAsync(updateGameDto));
    }

    [Fact]
    public async Task UpdateGameAsync_ValidProductData_ShouldCopyProductAndRelatedEntitiesToSql()
    {
        // Arrange
        var updateGameDto = GameServiceTestData.UpdateProductGameDto;
        var oldProduct = GameServiceTestData.OldProduct;
        var oldCategory = GameServiceTestData.OldCategory;
        var oldSupplier = GameServiceTestData.OldSupplier;

        _unitOfWorkMockMongo.Setup(u => u.Products
            .GetByIdAsync(int.Parse(updateGameDto.Game.Id), false)).ReturnsAsync(oldProduct);
        _unitOfWorkMockMongo.Setup(u => u.Categories
            .GetByIdAsync(oldCategory.CategoryID, true)).ReturnsAsync(oldCategory);
        _unitOfWorkMockMongo.Setup(u => u.Suppliers
            .GetByIdAsync(oldSupplier.SupplierID, true)).ReturnsAsync(oldSupplier);

        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(oldProduct.Key, false)).ReturnsAsync((Game)null);
        _unitOfWorkMock.Setup(u => u.Platforms.IdExist(updateGameDto.Platforms.First(), true)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Genres.IdExist(It.IsAny<Guid>(), true)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Publishers.IdExist(It.IsAny<Guid>(), false)).Returns(true);

        // Act
        await _gameService.UpdateGameAsync(updateGameDto);

        // Assert
        _unitOfWorkMock.Verify(u => u.Games.AddAsync(It.IsAny<Game>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Genres.AddAsync(It.IsAny<Genre>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Publishers.AddAsync(It.IsAny<Publisher>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGameAsync_ValidCopiedProductData_ShouldUpdateCopiedProduct()
    {
        // Arrange
        var updateGameDto = GameServiceTestData.UpdateProductGameDto;
        var oldProduct = GameServiceTestData.OldCopiedProduct;
        var oldCategory = GameServiceTestData.OldCopiedCategory;
        var oldSupplier = GameServiceTestData.OldCopiedSupplier;
        var copiedProductGame = GameServiceTestData.CopiedProduct;

        _unitOfWorkMockMongo.Setup(u => u.Products
            .GetByIdAsync(int.Parse(updateGameDto.Game.Id), false)).ReturnsAsync(oldProduct);
        _unitOfWorkMockMongo.Setup(u => u.Categories
            .GetByIdAsync(oldCategory.CategoryID, true)).ReturnsAsync(oldCategory);
        _unitOfWorkMockMongo.Setup(u => u.Suppliers
            .GetByIdAsync(oldSupplier.SupplierID, true)).ReturnsAsync(oldSupplier);

        _unitOfWorkMock.Setup(u => u.Games.GetByProductIdAsync(oldProduct.ProductID!.Value, false)).ReturnsAsync(copiedProductGame);
        _unitOfWorkMock.Setup(u => u.Games.GetByIdAsync(It.IsAny<Guid>(), false)).ReturnsAsync(copiedProductGame);
        _unitOfWorkMock.Setup(u => u.Genres.GetByCategoryIdAsync(oldCategory.CategoryID, true))
            .ReturnsAsync(new Genre { Id = Guid.NewGuid() });
        _unitOfWorkMock.Setup(u => u.Publishers.GetBySupplierIdAsync(oldSupplier.SupplierID, true))
            .ReturnsAsync(new Publisher { Id = Guid.NewGuid() });
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(oldProduct.Key, false)).ReturnsAsync((Game)null);
        _unitOfWorkMock.Setup(u => u.Platforms.IdExist(updateGameDto.Platforms.First(), true)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Genres.IdExist(It.IsAny<Guid>(), true)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Publishers.IdExist(It.IsAny<Guid>(), false)).Returns(true);

        // Act
        await _gameService.UpdateGameAsync(updateGameDto);

        // Assert
        _unitOfWorkMock.Verify(u => u.Games.Update(It.IsAny<Game>()), Times.Once);
    }

    [Fact]
    public async Task GetAllGamesAsync_ReturnsListOfGames()
    {
        // Arrange
        var games = GameServiceTestData.GamesList;
        var products = GameServiceTestData.ProductsList;

        _unitOfWorkMock.Setup(u => u.Games.GetAllAsync(false))
            .ReturnsAsync(games);
        _unitOfWorkMockMongo.Setup(u => u.Products.GetAllAsync(false))
            .ReturnsAsync(products);

        // Act
        var result = await _gameService.GetAllGamesAsync();

        // Assert
        Assert.Equal(games.Count + products.Count, result.Count());
    }

    [Fact]
    public async Task GetGameByIdAsync_InvalidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Games.GetByIdAsync(It.IsAny<Guid>(), false))
            .ReturnsAsync((Game)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
        _gameService.GetGameByIdAsync("77777777-7777-7777-7777-777777777777"));
    }

    [Fact]
    public async Task GetGameByIdAsync_InvalidProductId_ThrowsEntityNotFoundException()
    {
        // Arrange
        _unitOfWorkMockMongo.Setup(u => u.Products.GetByIdAsync(It.IsAny<int>(), false))
            .ReturnsAsync((Product)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
        _gameService.GetGameByIdAsync("1"));
    }

    [Fact]
    public async Task GetGameByIdAsync_ValidId_ReturnsGame()
    {
        // Arrange
        var validId = GameServiceTestData.GameEntity.Id;
        _unitOfWorkMock.Setup(u => u.Games.GetByIdAsync(validId, false))
            .ReturnsAsync(GameServiceTestData.GameEntity);

        // Act
        var result = await _gameService.GetGameByIdAsync(validId.ToString());

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetGameByIdAsync_ValidProductId_ReturnsProduct()
    {
        // Arrange
        var validId = GameServiceTestData.ProductEntity.ProductID!.Value;
        _unitOfWorkMockMongo.Setup(u => u.Products.GetByIdAsync(validId, false))
            .ReturnsAsync(GameServiceTestData.ProductEntity);

        // Act
        var result = await _gameService.GetGameByIdAsync(validId.ToString());

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetGameByIdAsync_InvalidIdFormat_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
        _gameService.GetGameByIdAsync("1-5"));
    }

    [Fact]
    public async Task GetGameByKeyAsync_InvalidKey_ThrowsEntityNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(It.IsAny<string>(), false))
            .ReturnsAsync((Game)null);
        _unitOfWorkMockMongo.Setup(u => u.Products.GetByKeyAsync(It.IsAny<string>(), false))
            .ReturnsAsync((Product)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.GetGameByKeyAsync(string.Empty));
    }

    [Fact]
    public async Task GetGameByKeyAsync_ValidKey_ReturnsGame()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(GameServiceTestData.GameEntity.Key, false))
            .ReturnsAsync(GameServiceTestData.GameEntity);

        // Act
        var result = await _gameService.GetGameByKeyAsync(GameServiceTestData.GameEntity.Key, false);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetGameByKeyAsync_ValidProductKey_ReturnsProduct()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(It.IsAny<string>(), false))
            .ReturnsAsync((Game)null);
        _unitOfWorkMockMongo.Setup(u => u.Products.GetByKeyAsync(GameServiceTestData.ProductEntity.Key, false))
            .ReturnsAsync(GameServiceTestData.ProductEntity);

        // Act
        var result = await _gameService.GetGameByKeyAsync(GameServiceTestData.ProductEntity.Key);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetGamesByGenreAsync_ValidGuidId_ReturnsListOfGames()
    {
        // Arrange
        var games = GameServiceTestData.GamesList;

        var genreId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Games.GetByGenreAsync(genreId, false))
            .ReturnsAsync(games);

        // Act
        var result = await _gameService.GetGamesByGenreAsync(genreId.ToString());

        // Assert
        Assert.Equal(games.Count, result.Count());
    }

    [Fact]
    public async Task GetGamesByGenreAsync_ValidIntId_ReturnsListOfGames()
    {
        // Arrange
        var products = GameServiceTestData.ProductsList;

        var genreId = 1;
        _unitOfWorkMockMongo.Setup(u => u.Products.GetByCategoryAsync(genreId, false))
            .ReturnsAsync(products);

        // Act
        var result = await _gameService.GetGamesByGenreAsync(genreId.ToString());

        // Assert
        Assert.Equal(products.Count, result.Count());
    }

    [Fact]
    public async Task GetGamesByGenreAsync_InvalidIdFomat_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _gameService.GetGamesByGenreAsync("1-5-1"));
    }

    [Fact]
    public async Task GetGamesByPublisherAsync_ReturnsListOfGames()
    {
        // Arrange
        var games = GameServiceTestData.GamesList;
        var products = GameServiceTestData.ProductsList;
        var companyName = "Company name";

        _unitOfWorkMock.Setup(u => u.Games.GetByPublisherAsync(companyName, false))
            .ReturnsAsync(games);
        _unitOfWorkMockMongo.Setup(u => u.Products.GetBySupplierNameAsync(companyName, false))
            .ReturnsAsync(products);

        // Act
        var result = await _gameService.GetGamesByPublisherAsync(companyName);

        // Assert
        Assert.Equal(games.Count + products.Count, result.Count());
    }

    [Fact]
    public async Task GetGamesByPlatformAsync_ReturnsListOfGames()
    {
        // Arrange
        var games = GameServiceTestData.GamesList;

        var platformId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Games.GetByPlatformAsync(platformId, false))
            .ReturnsAsync(games);

        // Act
        var result = await _gameService.GetGamesByPlatformAsync(platformId);

        // Assert
        Assert.Equal(games.Count, result.Count());
    }

    [Theory]
    [MemberData(
        nameof(GameServiceTestData.GetGamesAsyncTestData),
        MemberType = typeof(GameServiceTestData))]
    public void GetGames_ReturnsExpectedResult(
        GameQuery query,
        IEnumerable<Game> gameEntities,
        IEnumerable<Product> productEntities,
        PagedGames expected)
    {
        // Arrange
        _unitOfWorkMock.Setup(uow => uow.Games.GetGamesQuery(false)).Returns(gameEntities.AsQueryable());
        _unitOfWorkMockMongo.Setup(u => u.Products.GetQuery(false)).Returns(productEntities.AsQueryable());

        var cacheEntryMock = new Mock<ICacheEntry>();
        _memoryCacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
                  .Returns(false); // Simulate cache miss

        _memoryCacheMock.Setup(c => c.CreateEntry(It.IsAny<object>()))
                  .Returns(cacheEntryMock.Object);

        // Act
        var result = _gameService.GetGames(query);

        // Assert
        result.Games.Should().BeEquivalentTo(expected.Games, options => options.WithStrictOrdering());
        Assert.Equal(expected.CurrentPage, result.CurrentPage);
        Assert.Equal(expected.TotalPages, result.TotalPages);
    }
}