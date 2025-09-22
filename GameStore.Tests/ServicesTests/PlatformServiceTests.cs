using AutoMapper;
using GameStore.API.Mappings;
using GameStore.Application.Helpers;
using GameStore.Application.Services.PlatformServices;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Tests.TestUtilities.ServicesTestData;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameStore.Tests.ServicesTests;
public class PlatformServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PlatformService _platformService;

    public PlatformServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var mockLogger = new Mock<ILogger<EntityChangeLogger>>();
        var changeLogger = new EntityChangeLogger(mockLogger.Object);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();

        _platformService = new PlatformService(_unitOfWorkMock.Object, mapper, changeLogger);
    }

    [Fact]
    public async Task AddPlatformAsync_ValidPlatformDto_WorksCorrectly()
    {
        // Arrange
        var addPlatformDto = PlatformServiceTestData.AddPlatformDto;

        _unitOfWorkMock.Setup(u => u.Platforms.TypeExist(addPlatformDto.Type)).Returns(false);

        // Act
        await _platformService.AddPlatformAsync(addPlatformDto);

        // Assert
        _unitOfWorkMock.Verify(u => u.Platforms.AddAsync(It.IsAny<Platform>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task AddPlatformAsync_DuplicatePlatform_ThrowsUniqueConstraintException()
    {
        // Arrange
        var platformDto = PlatformServiceTestData.AddPlatformDto;

        // Simulate platform already exists
        _unitOfWorkMock.Setup(u => u.Platforms.TypeExist(platformDto.Type)).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<UniquePropertyException>(() => _platformService.AddPlatformAsync(platformDto));
    }

    [Fact]
    public async Task DeletePlatformAsync_ValidId_ShouldDeletePlatform()
    {
        // Arrange
        var id = Guid.NewGuid();
        var platform = new Platform { Id = id, Type = "Test type" };

        _unitOfWorkMock.Setup(u => u.Platforms.GetByIdAsync(id, false))
            .ReturnsAsync(platform);

        // Act
        await _platformService.DeletePlatformAsync(id);

        // Assert
        _unitOfWorkMock.Verify(u => u.Platforms.Delete(platform), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletePlatformAsync_InvalidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Platforms.GetByIdAsync(id, false))
            .ReturnsAsync((Platform)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _platformService.DeletePlatformAsync(id));
    }

    [Fact]
    public async Task UpdatePlatformAsync_ValidData_ShouldUpdatePlatform()
    {
        // Arrange
        var updatePlatformDto = PlatformServiceTestData.UpdatePlatformDto;

        _unitOfWorkMock.Setup(u => u.Platforms.GetByIdAsync(updatePlatformDto.Id, false))
            .ReturnsAsync(new Platform() { Id = updatePlatformDto.Id });

        // Act
        await _platformService.UpdatePlatformAsync(updatePlatformDto);

        // Assert
        _unitOfWorkMock.Verify(u => u.Platforms.Update(It.IsAny<Platform>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdatePlatformAsync_InvalidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var updatePlatformDto = PlatformServiceTestData.UpdatePlatformDto;

        _unitOfWorkMock.Setup(u => u.Platforms.GetByIdAsync(It.IsAny<Guid>(), false))
            .ReturnsAsync((Platform)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _platformService.UpdatePlatformAsync(updatePlatformDto));
    }

    [Fact]
    public async Task UpdatePlatformAsync_PlatformTypeExist_ThrowsUniquePropertyException()
    {
        // Arrange
        var updatePlatformDto = PlatformServiceTestData.UpdatePlatformDto;

        _unitOfWorkMock.Setup(u => u.Platforms.GetByIdAsync(updatePlatformDto.Id, false))
            .ReturnsAsync(new Platform() { Id = updatePlatformDto.Id, Type = "Another type" });
        _unitOfWorkMock.Setup(u => u.Platforms.TypeExist(updatePlatformDto.Type)).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<UniquePropertyException>(() => _platformService.UpdatePlatformAsync(updatePlatformDto));
    }

    [Fact]
    public async Task GetAllPlatformsAsync_ReturnsListOfPlaforms()
    {
        // Arrange
        var platforms = PlatformServiceTestData.PlatformList;

        _unitOfWorkMock.Setup(u => u.Platforms.GetAllAsync(false))
            .ReturnsAsync(platforms);

        // Act
        var result = await _platformService.GetAllPlatformsAsync();

        // Assert
        Assert.Equal(platforms.Count, result.Count());
    }

    [Fact]
    public async Task GetPlatformByIdAsync_InvalidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Platforms.GetByIdAsync(It.IsAny<Guid>(), false))
            .ReturnsAsync((Platform)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _platformService.GetPlatformByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetPlatformByIdAsync_ValidId_ReturnsPlatform()
    {
        // Arrange
        var validId = PlatformServiceTestData.PlatformEntity.Id;
        _unitOfWorkMock.Setup(u => u.Platforms.GetByIdAsync(validId, false))
            .ReturnsAsync(PlatformServiceTestData.PlatformEntity);

        // Act
        var result = await _platformService.GetPlatformByIdAsync(validId);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPlatformsByGameKeyAsync_ValidKey_ReturnsPlatformList()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Platforms.GetByGameKeyAsync("Test-key", false))
            .ReturnsAsync(PlatformServiceTestData.PlatformList);

        // Act
        var result = await _platformService.GetPlatformsByGameKeyAsync("Test-key");

        // Assert
        Assert.NotEmpty(result);
    }
}