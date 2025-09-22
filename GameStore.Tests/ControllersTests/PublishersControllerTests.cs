using System.Security.Claims;
using AutoMapper;
using GameStore.API.Controllers;
using GameStore.API.Mappings;
using GameStore.API.Models.PublisherModels;
using GameStore.Application.DTOs.PublisherDtos;
using GameStore.Application.Services.PublisherService;
using GameStore.Domain.Constants;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.ControllersTestData;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests.ControllersTests;
public class PublishersControllerTests
{
    private readonly Mock<IPublisherService> _publisherServiceMock;
    private readonly PublishersController _controller;

    public PublishersControllerTests()
    {
        _publisherServiceMock = new Mock<IPublisherService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();

        _controller = new PublishersController(_publisherServiceMock.Object, mapper);
        ControllerTestHelpers.SetUserClaims(
            [
                new(ClaimTypes.NameIdentifier, "test-user-id"),
                new(Permissions.PermissionClaim, Permissions.ViewDeletedGames),
            ],
            _controller);
    }

    [Fact]
    public async Task AddPublisher_ValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var addPublisherModel = new AddPublisherModel
        {
            CompanyName = "Publisher name",
            Description = "descrition",
            HomePage = "http://www.google.com",
        };
        var requestModel = new AddPublisherRequestModel { Publisher = addPublisherModel };
        var publisherDto = new PublisherDto { CompanyName = "Publisher name" };

        _publisherServiceMock.Setup(service => service.AddPublisherAsync(It.IsAny<AddPublisherDto>())).ReturnsAsync(publisherDto);

        // Act
        var result = await _controller.AddPublisher(requestModel);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task AddPublisher_NullInput_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.AddPublisher(null);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddPublisher_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var addPublisherModel = new AddPublisherModel();
        var requestModel = new AddPublisherRequestModel { Publisher = addPublisherModel };

        _controller.ModelState.AddModelError("CompanyName", "The CompanyName field is required.");

        // Act
        var result = await _controller.AddPublisher(requestModel);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetPublisherByName_ExistingPublisher_ReturnsOk()
    {
        // Arrange
        var publisherDto = PublishersControllerTestData.PublisherDto;
        var publisherName = publisherDto.CompanyName;
        _publisherServiceMock.Setup(service => service.GetPublisherByNameAsync(publisherName, false)).ReturnsAsync(publisherDto);

        // Act
        var result = await _controller.GetPublisherByName(publisherName);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetPublisherByName_ExistingPlatform_CorrectlyMappsDtoToModel()
    {
        // Arrange
        var publisherDto = PublishersControllerTestData.PublisherDto;
        var publisherName = publisherDto.CompanyName;
        _publisherServiceMock.Setup(service => service.GetPublisherByNameAsync(publisherName, true)).ReturnsAsync(publisherDto);

        // Act
        var result = await _controller.GetPublisherByName(publisherName);

        // Assert
        var okResult = result as OkObjectResult;
        var resultModel = okResult.Value as PublisherModel;
        Assert.Equal(publisherDto.Id, resultModel.Id);
        Assert.Equal(publisherDto.CompanyName, resultModel.CompanyName);
    }

    [Fact]
    public async Task GetAllPublishers_ReturnsOkResult()
    {
        // Arrange
        var publishersList = PublishersControllerTestData.ListOfPublisherDtos;
        _publisherServiceMock.Setup(service => service.GetAllPublishersAsync(false)).ReturnsAsync(publishersList);

        // Act
        var result = await _controller.GetAllPublishers();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetPublisherByGameKey_ExistingPlatform_ReturnsOk()
    {
        // Arrange
        var gameKey = "valid key";
        var publisherDto = PublishersControllerTestData.PublisherDto;
        _publisherServiceMock.Setup(service => service.GetPublisherByGameKeyAsync(gameKey, false)).ReturnsAsync(publisherDto);

        // Act
        var result = await _controller.GetPublisherByGameKey(gameKey);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetPublisherByGameKey_ExistingPlatform_CorrectlyMappsDtoToModel()
    {
        // Arrange
        var gameKey = "valid key";
        var publisherDto = PublishersControllerTestData.PublisherDto;
        _publisherServiceMock.Setup(service => service.GetPublisherByGameKeyAsync(gameKey, true)).ReturnsAsync(publisherDto);

        // Act
        var result = await _controller.GetPublisherByGameKey(gameKey);

        // Assert
        var okResult = result as OkObjectResult;
        var resultModel = okResult.Value as PublisherModel;
        Assert.Equal(publisherDto.Id, resultModel.Id);
        Assert.Equal(publisherDto.CompanyName, resultModel.CompanyName);
    }

    [Fact]
    public async Task UpdatePublisher_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var publisherModel = new PublisherModel();
        var requestModel = new UpdatePublisherRequestModel { Publisher = publisherModel };

        // Simulate ModelState invalidation
        _controller.ModelState.AddModelError("CompanyName", "The CompanyName field is required.");

        // Act
        var result = await _controller.UpdatePublisher(requestModel);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdatePublisher_ValidModel_ReturnsOk()
    {
        // Arrange
        var updatePublisherModel = new UpdatePublisherRequestModel()
        {
            Publisher = new PublisherModel()
            {
                Id = Guid.NewGuid().ToString(),
                CompanyName = "Updated Name",
                Description = "Updated description",
                HomePage = "http://www.google.com",
            },
        };

        _publisherServiceMock.Setup(service => service.UpdatePublisherAsync(It.IsAny<PublisherDto>(), false)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdatePublisher(updatePublisherModel);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeletePublisher_ExistingPublisher_ReturnsNoContent()
    {
        // Arrange
        var publisherId = Guid.NewGuid().ToString();
        _publisherServiceMock.Setup(service => service.DeletePublisherAsync(publisherId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeletePublisher(publisherId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
