using AutoMapper;
using GameStore.API.Mappings;
using GameStore.Application.Helpers;
using GameStore.Application.Services.PublisherService;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Tests.TestUtilities.ServicesTestData;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameStore.Tests.ServicesTests;
public class PublisherServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PublisherService _publisherService;
    private readonly Mock<ISupplierRepository> _supplierRepositoryMock;

    public PublisherServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _supplierRepositoryMock = new Mock<ISupplierRepository>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        var mockLogger = new Mock<ILogger<EntityChangeLogger>>();
        var changeLogger = new EntityChangeLogger(mockLogger.Object);

        _publisherService = new PublisherService(
            _unitOfWorkMock.Object,
            _supplierRepositoryMock.Object,
            mapper,
            changeLogger);
    }

    [Fact]
    public async Task AddPublisherAsync_ValidPublisherDto_WorksCorrectly()
    {
        // Arrange
        var addPublisherDto = PublisherServiceTestData.AddPublisherDto;

        _unitOfWorkMock.Setup(u => u.Publishers.NameExist(addPublisherDto.CompanyName)).Returns(false);

        // Act
        await _publisherService.AddPublisherAsync(addPublisherDto);

        // Assert
        _unitOfWorkMock.Verify(u => u.Publishers.AddAsync(It.IsAny<Publisher>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task AddPublisherAsync_DuplicatePublisher_ThrowsUniqueConstraintException()
    {
        // Arrange
        var addPublisherDto = PublisherServiceTestData.AddPublisherDto;

        // Simulate platform already exists
        _unitOfWorkMock.Setup(u => u.Publishers.NameExist(addPublisherDto.CompanyName)).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<UniquePropertyException>(() => _publisherService.AddPublisherAsync(addPublisherDto));
    }

    [Fact]
    public async Task DeletePublisherAsync_ValidId_ShouldDeletePublisher()
    {
        // Arrange
        string id = "11111111-1111-1111-1111-111111111111";
        var guidId = Guid.Parse(id);
        var publisher = new Publisher { Id = guidId, CompanyName = "Publisher to delete" };

        _unitOfWorkMock.Setup(u => u.Publishers.GetByIdAsync(guidId, false))
            .ReturnsAsync(publisher);

        // Act
        await _publisherService.DeletePublisherAsync(id);

        // Assert
        _unitOfWorkMock.Verify(u => u.Publishers.Delete(publisher), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletePublisherAsync_InvalidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        string id = "11111111-1111-1111-1111-111111111111";
        var guidId = Guid.Parse(id);
        _unitOfWorkMock.Setup(u => u.Publishers.GetByIdAsync(guidId, false))
            .ReturnsAsync((Publisher)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _publisherService.DeletePublisherAsync(id));
    }

    [Fact]
    public async Task GetAllPublishersAsync_ReturnsListOfPlaforms()
    {
        // Arrange
        var publishers = PublisherServiceTestData.PublisherList;

        _unitOfWorkMock.Setup(u => u.Publishers.GetAllAsync(false))
            .ReturnsAsync(publishers);

        // Act
        var result = await _publisherService.GetAllPublishersAsync();

        // Assert
        Assert.Equal(publishers.Count, result.Count());
    }

    [Fact]
    public async Task GetPublisherByGameKeyAsync_InvalidKey_ThrowsEntityNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Publishers.GetByGameKeyAsync(It.IsAny<string>(), false))
            .ReturnsAsync((Publisher)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _publisherService.GetPublisherByGameKeyAsync("Invalid key"));
    }

    [Fact]
    public async Task GetPublisherByGameKeyAsync_ValidKey_ReturnsPublisher()
    {
        // Arrange
        var validKey = "valid key";
        _unitOfWorkMock.Setup(u => u.Publishers.GetByGameKeyAsync(validKey, false))
            .ReturnsAsync(PublisherServiceTestData.Publisher);

        // Act
        var result = await _publisherService.GetPublisherByGameKeyAsync(validKey);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPublisherByNameAsync_InvalidName_ThrowsEntityNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Publishers.GetByCompanyNameAsync(It.IsAny<string>(), false))
            .ReturnsAsync((Publisher)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _publisherService.GetPublisherByNameAsync("Invalid key"));
    }

    [Fact]
    public async Task GetPublisherByNameAsync_ValidName_ReturnsPublisher()
    {
        // Arrange
        var validName = PublisherServiceTestData.Publisher.CompanyName;
        _unitOfWorkMock.Setup(u => u.Publishers.GetByCompanyNameAsync(validName, false))
            .ReturnsAsync(PublisherServiceTestData.Publisher);

        // Act
        var result = await _publisherService.GetPublisherByNameAsync(validName);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdatePublisherAsync_ValidData_ShouldUpdatePublisher()
    {
        // Arrange
        var publisherDto = PublisherServiceTestData.PublisherDto;
        var guidId = Guid.Parse(publisherDto.Id);

        _unitOfWorkMock.Setup(u => u.Publishers.GetByIdAsync(guidId, false))
            .ReturnsAsync(new Publisher() { Id = guidId });

        // Act
        await _publisherService.UpdatePublisherAsync(publisherDto);

        // Assert
        _unitOfWorkMock.Verify(u => u.Publishers.Update(It.IsAny<Publisher>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdatePublisherAsync_InvalidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var publisherDto = PublisherServiceTestData.PublisherDto;
        var guidId = Guid.Parse(publisherDto.Id);

        _unitOfWorkMock.Setup(u => u.Publishers.GetByIdAsync(guidId, false))
            .ReturnsAsync((Publisher)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _publisherService.UpdatePublisherAsync(publisherDto));
    }

    [Fact]
    public async Task UpdatePublisherAsync_PublisherNameExist_ThrowsUniquePropertyException()
    {
        // Arrange
        var publisherDto = PublisherServiceTestData.PublisherDto;
        var guidId = Guid.Parse(publisherDto.Id);

        _unitOfWorkMock.Setup(u => u.Publishers.GetByIdAsync(guidId, false))
            .ReturnsAsync(new Publisher() { Id = guidId, CompanyName = "Another name" });
        _unitOfWorkMock.Setup(u => u.Publishers.NameExist(publisherDto.CompanyName)).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<UniquePropertyException>(() => _publisherService.UpdatePublisherAsync(publisherDto));
    }
}
