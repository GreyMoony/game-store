using System.Security.Claims;
using AutoMapper;
using GameStore.API.Controllers;
using GameStore.API.Mappings;
using GameStore.API.Models.PlatformModels;
using GameStore.Application.DTOs.PlatformDtos;
using GameStore.Application.Services.PlatformServices;
using GameStore.Domain.Constants;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.ControllersTestData;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests.ControllersTests;
public class PlatformsControllerTests
{
    private readonly Mock<IPlatformService> _platformServiceMock;
    private readonly PlatformsController _controller;

    public PlatformsControllerTests()
    {
        _platformServiceMock = new Mock<IPlatformService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();

        _controller = new PlatformsController(_platformServiceMock.Object, mapper);
        ControllerTestHelpers.SetUserClaims(
            [
                new(ClaimTypes.NameIdentifier, "test-user-id"),
                new(Permissions.PermissionClaim, Permissions.ViewDeletedGames),
            ],
            _controller);
    }

    [Fact]
    public async Task AddPlatform_ValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var addPlatformModel = new AddPlatformModel { Type = "test platform" };
        var requestModel = new AddPlatformRequestModel { Platform = addPlatformModel };
        var platformDto = new PlatformDto { Type = "test platform" };

        _platformServiceMock.Setup(service => service.AddPlatformAsync(It.IsAny<AddPlatformDto>())).ReturnsAsync(platformDto);

        // Act
        var result = await _controller.AddPlatform(requestModel);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task AddPlatform_NullInput_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.AddPlatform(null);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddPlatform_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var addPlatformModel = new AddPlatformModel();
        var requestModel = new AddPlatformRequestModel { Platform = addPlatformModel };

        _controller.ModelState.AddModelError("Type", "The Type field is required.");

        // Act
        var result = await _controller.AddPlatform(requestModel);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAllPlatforms_ReturnsOkResult()
    {
        // Arrange
        var platformList = PlatformsControllerTestData.ListOfPlatformDtos;
        _platformServiceMock.Setup(service => service.GetAllPlatformsAsync(false)).ReturnsAsync(platformList);

        // Act
        var result = await _controller.GetAllPlatforms();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetPlatformById_ExistingPlatform_ReturnsOk()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        var platformDto = new PlatformDto { Id = platformId, Type = "test platform" };
        _platformServiceMock.Setup(service => service.GetPlatformByIdAsync(platformId, false)).ReturnsAsync(platformDto);

        // Act
        var result = await _controller.GetPlatformById(platformId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetPlatformById_ExistingPlatform_CorrectlyMappsDtoToModel()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        var platformDto = new PlatformDto { Id = platformId, Type = "test platform" };
        _platformServiceMock.Setup(service => service.GetPlatformByIdAsync(platformId, true)).ReturnsAsync(platformDto);

        // Act
        var result = await _controller.GetPlatformById(platformId);

        // Assert
        var okResult = result as OkObjectResult;
        var resultModel = okResult.Value as PlatformModel;
        Assert.Equal(platformDto.Id, resultModel.Id);
        Assert.Equal(platformDto.Type, resultModel.Type);
    }

    [Fact]
    public async Task UpdatePlatform_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var platformModel = new PlatformModel();
        var requestModel = new UpdatePlatformModel { Platform = platformModel };

        // Simulate ModelState invalidation
        _controller.ModelState.AddModelError("Type", "The Type field is required.");

        // Act
        var result = await _controller.UpdatePlatform(requestModel);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdatePlatform_ValidModel_ReturnsOk()
    {
        // Arrange
        var updatePlatformModel = new UpdatePlatformModel()
        {
            Platform = new PlatformModel()
            {
                Id = Guid.NewGuid(),
                Type = "Updated Name",
            },
        };

        _platformServiceMock.Setup(service => service.UpdatePlatformAsync(It.IsAny<PlatformDto>(), false)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdatePlatform(updatePlatformModel);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeletePlatform_ExistingPlatform_ReturnsNoContent()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        _platformServiceMock.Setup(service => service.DeletePlatformAsync(platformId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeletePlatform(platformId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
