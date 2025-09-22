using System.Security.Claims;
using AutoMapper;
using GameStore.API.Controllers;
using GameStore.API.Helpers.RequestContext;
using GameStore.API.Mappings;
using GameStore.API.Models.GameModels;
using GameStore.Application.DTOs.GameDtos;
using GameStore.Application.Services.GameServices;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.ControllersTestData;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests.ControllersTests;
public class GamesControllerTests
{
    private const string Lang = "en";
    private readonly Mock<IGameService> _gameServiceMock;
    private readonly GamesController _controller;

    public GamesControllerTests()
    {
        _gameServiceMock = new Mock<IGameService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        var locContextMock = new Mock<IRequestLocalizationContext>();

        locContextMock.Setup(l => l.CurrentLanguage).Returns(Lang);
        _controller = new GamesController(_gameServiceMock.Object, mapper, locContextMock.Object);
        ControllerTestHelpers.SetUserClaims(
            [
                new(ClaimTypes.NameIdentifier, "test-user-id"),
                new(Permissions.PermissionClaim, Permissions.ViewDeletedGames),
            ],
            _controller);
    }

    [Fact]
    public async Task AddGame_ValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var addGameModel = new AddGameModel { Game = new ShortGameModel { Name = "Test Game", Key = "test-game" } };
        var gameDto = new GameDto { Name = "Test Game", Key = "test-game" };

        _gameServiceMock.Setup(service => service.AddGameAsync(It.IsAny<AddGameDto>())).ReturnsAsync(gameDto);

        // Act
        var result = await _controller.AddGame(addGameModel);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task AddGame_NullInput_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.AddGame(null);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddGame_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var addGameModel = new AddGameModel(); // Invalid Model, missing required fields

        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var result = await _controller.AddGame(addGameModel);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetGameByKey_ExistingGame_ReturnsOk()
    {
        // Arrange
        var gameKey = GamesControllerTestData.GameDto.Key;
        var gameDto = GamesControllerTestData.GameDto;
        _gameServiceMock.Setup(service => service.GetGameByKeyAsync(gameKey, false)).ReturnsAsync(gameDto);

        // Act
        var result = await _controller.GetGameByKey(gameKey);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetGameByKey_ExistingGame_CorrectlyMapDtoToModel()
    {
        // Arrange
        var gameKey = GamesControllerTestData.GameDto.Key;
        var gameDto = GamesControllerTestData.GameDto;
        _gameServiceMock.Setup(service => service.GetLocalizedGameAsync(gameKey, Lang, true)).ReturnsAsync(gameDto);

        // Act
        var result = await _controller.GetGameByKey(gameKey);

        // Assert
        var okResult = result as OkObjectResult;
        var resultModel = okResult.Value as GameModel;
        Assert.Equal(gameDto.Name, resultModel.Name);
        Assert.Equal(gameDto.Key, resultModel.Key);
        Assert.Equal(gameDto.Description, resultModel.Description);
    }

    [Fact]
    public async Task GetGamesByPlatform_ReturnsOkResult()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        var gamesList = GamesControllerTestData.ListOfGameDtos;

        _gameServiceMock.Setup(service => service.GetGamesByPlatformAsync(platformId, false)).ReturnsAsync(gamesList);

        // Act
        var result = await _controller.GetGamesByPlatform(platformId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetGamesByPlatform_ReturnsListOfGames()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        var gamesList = GamesControllerTestData.ListOfGameDtos;

        _gameServiceMock.Setup(service => service.GetGamesByPlatformAsync(platformId, false)).ReturnsAsync(gamesList);

        // Act
        var result = await _controller.GetGamesByPlatform(platformId);

        // Assert
        Assert.IsType<List<GameModel>>((result as OkObjectResult).Value);
    }

    [Fact]
    public async Task UpdateGame_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var updateGameModel = new UpdateGameModel();

        // Simulate ModelState invalidation
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var result = await _controller.UpdateGame(updateGameModel);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateGame_ValidModel_ReturnsOk()
    {
        // Arrange
        var updateGameModel = new UpdateGameModel()
        {
            Game = new GameModel()
            {
                Id = "77777777-7777-7777-7777-77777777777",
                Name = "Updated Name",
                Description = "Updated description",
                Key = "updated key",
            },
        };

        _gameServiceMock.Setup(service => service.UpdateGameAsync(It.IsAny<UpdateGameDto>(), false))
            .ReturnsAsync(new Game { Id = Guid.NewGuid() });

        // Act
        var result = await _controller.UpdateGame(updateGameModel);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteGame_ExistingGame_ReturnsNoContent()
    {
        // Arrange
        var gameKey = "test-game";
        _gameServiceMock.Setup(service => service.DeleteGameAsync(gameKey)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteGame(gameKey);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
