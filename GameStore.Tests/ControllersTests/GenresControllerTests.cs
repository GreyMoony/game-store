using System.Security.Claims;
using AutoMapper;
using GameStore.API.Controllers;
using GameStore.API.Mappings;
using GameStore.API.Models.GenreModels;
using GameStore.Application.DTOs.GenreDtos;
using GameStore.Application.Services.GenreServices;
using GameStore.Domain.Constants;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.ControllersTestData;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests.ControllersTests;
public class GenresControllerTests
{
    private readonly Mock<IGenreService> _genreServiceMock;
    private readonly GenresController _controller;

    public GenresControllerTests()
    {
        _genreServiceMock = new Mock<IGenreService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();

        _controller = new GenresController(_genreServiceMock.Object, mapper);
        ControllerTestHelpers.SetUserClaims(
            [
                new(ClaimTypes.NameIdentifier, "test-user-id"),
                new(Permissions.PermissionClaim, Permissions.ViewDeletedGames),
            ],
            _controller);
    }

    [Fact]
    public async Task AddGenre_ValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var addGenreModel = new AddGenreModel { Name = "test genre" };
        var requestModel = new AddGenreRequestModel { Genre = addGenreModel };
        var genreDto = new GenreDto { Name = "test genre" };

        _genreServiceMock.Setup(service => service.AddGenreAsync(It.IsAny<AddGenreDto>())).ReturnsAsync(genreDto);

        // Act
        var result = await _controller.AddGenre(requestModel);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task AddGenre_NullInput_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.AddGenre(null);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddGenre_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var addGenreModel = new AddGenreModel(); // Invalid DTO, missing required fields
        var requestModel = new AddGenreRequestModel { Genre = addGenreModel };

        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var result = await _controller.AddGenre(requestModel);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAllGenres_ReturnsOkResult()
    {
        // Arrange
        var genreList = GenresControllerTestData.ListOfGenreDtos;
        _genreServiceMock.Setup(service => service.GetAllGenresAsync(false)).ReturnsAsync(genreList);

        // Act
        var result = await _controller.GetAllGenres();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAllGenres_ReturnsListOfGenres()
    {
        // Arrange
        var genreList = GenresControllerTestData.ListOfGenreDtos;
        _genreServiceMock.Setup(service => service.GetAllGenresAsync(false)).ReturnsAsync(genreList);

        // Act
        var result = await _controller.GetAllGenres();

        // Assert
        Assert.IsType<List<ShortGenreModel>>((result as OkObjectResult).Value);
    }

    [Fact]
    public async Task GetGenreById_ExistingGenre_ReturnsOk()
    {
        // Arrange
        var genreId = Guid.NewGuid().ToString();
        var genreDto = new GenreDto { Id = genreId, Name = "test genre" };
        _genreServiceMock.Setup(service => service.GetGenreByIdAsync(genreId, false)).ReturnsAsync(genreDto);

        // Act
        var result = await _controller.GetGenreById(genreId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetGenreById_ExistingGenre_CorrectlyMapDtoToModel()
    {
        // Arrange
        var genreId = Guid.NewGuid().ToString();
        var genreDto = new GenreDto { Id = genreId, Name = "test genre" };
        _genreServiceMock.Setup(service => service.GetGenreByIdAsync(genreId, true)).ReturnsAsync(genreDto);

        // Act
        var result = await _controller.GetGenreById(genreId);

        // Assert
        var okResult = result as OkObjectResult;
        var resultModel = okResult.Value as GenreModel;
        Assert.Equal(genreDto.Name, resultModel.Name);
        Assert.Equal(genreDto.Id, resultModel.Id);
    }

    [Fact]
    public async Task UpdateGenre_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var genreModel = new GenreModel();
        var requestModel = new UpdateGenreModel { Genre = genreModel };

        // Simulate ModelState invalidation
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var result = await _controller.UpdateGenre(requestModel);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateGenre_ValidModel_ReturnsOk()
    {
        // Arrange
        var updateGenreModel = new UpdateGenreModel()
        {
            Genre = new GenreModel()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Updated Name",
            },
        };

        _genreServiceMock.Setup(service => service.UpdateGenreAsync(It.IsAny<GenreDto>(), false)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateGenre(updateGenreModel);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteGenre_ExistingGenre_ReturnsNoContent()
    {
        // Arrange
        var genreId = Guid.NewGuid().ToString();
        _genreServiceMock.Setup(service => service.DeleteGenreAsync(genreId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteGenre(genreId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
