using AutoMapper;
using GameStore.API.Mappings;
using GameStore.Application.Helpers;
using GameStore.Application.Services.GenreServices;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Exceptions;
using GameStore.Tests.TestUtilities.ServicesTestData;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameStore.Tests.ServicesTests;
public class GenreServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly GenreService _genreService;

    public GenreServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        var mockLogger = new Mock<ILogger<EntityChangeLogger>>();
        var changeLogger = new EntityChangeLogger(mockLogger.Object);

        _genreService = new GenreService(
            _unitOfWorkMock.Object,
            _categoryRepositoryMock.Object,
            mapper,
            changeLogger);
    }

    [Fact]
    public async Task AddGenreAsync_ValidDto_WorksCorrectly()
    {
        // Arrange
        var addGenreDto = GenreServiceTestData.AddGenreDto;
        var parentGenre = new Genre()
        {
            Id = Guid.Parse(GenreServiceTestData.ValidParentGenreId),
            Name = "Parent",
        };

        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(parentGenre.Id, true))
            .ReturnsAsync(parentGenre);
        _unitOfWorkMock.Setup(u => u.Genres.NameExist(addGenreDto.Name)).Returns(false);

        // Act
        await _genreService.AddGenreAsync(addGenreDto);

        // Assert
        _unitOfWorkMock.Verify(u => u.Genres.AddAsync(It.IsAny<Genre>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task AddGenreAsync_InvalidParentGenreId_ThrowsIdsNotValidException()
    {
        // Arrange
        var genreDto = GenreServiceTestData.AddGenreDto;
        var guidId = Guid.Parse(genreDto.ParentGenreId!);

        _unitOfWorkMock.Setup(u => u.Genres.NameExist(genreDto.Name)).Returns(false);
        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(guidId, false))
            .ReturnsAsync((Genre)null);

        // Act & Assert
        await Assert.ThrowsAsync<IdsNotValidException>(() => _genreService.AddGenreAsync(genreDto));
    }

    [Fact]
    public async Task AddGenreAsync_DuplicateGenre_ThrowsUniqueConstraintException()
    {
        // Arrange
        var genreDto = GenreServiceTestData.AddGenreDto;

        // Simulate platform already exists
        _unitOfWorkMock.Setup(u => u.Genres.NameExist(genreDto.Name)).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<UniquePropertyException>(() => _genreService.AddGenreAsync(genreDto));
    }

    [Fact]
    public async Task AddGenreAsync_DtoWithCategoryID_AddsGenreWithCopiedParentCategory()
    {
        // Arrange
        var addGenreDto = GenreServiceTestData.AddGenreDtoWithParentCategory;
        var parentGenre = new Category()
        {
            CategoryID = int.Parse(addGenreDto.ParentGenreId!),
            CategoryName = "Parent",
        };

        _unitOfWorkMock.Setup(u => u.Genres.NameExist(addGenreDto.Name)).Returns(false);
        _categoryRepositoryMock.Setup(c =>
            c.GetByIdAsync(parentGenre.CategoryID, true)).ReturnsAsync(parentGenre);
        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(It.IsAny<Guid>(), true))
            .ReturnsAsync(new Genre { CategoryID = parentGenre.CategoryID });

        // Act
        await _genreService.AddGenreAsync(addGenreDto);

        // Assert
        _unitOfWorkMock.Verify(u => u.Genres.AddAsync(It.IsAny<Genre>()), Times.Exactly(2));
    }

    [Fact]
    public async Task DeleteGenreAsync_ValidGuidId_ShouldDeleteGenre()
    {
        // Arrange
        var subGenres = new List<Genre>();
        var genre = new Genre { Id = Guid.NewGuid(), Name = "Test genre name", SubGenres = subGenres };

        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(genre.Id, false))
            .ReturnsAsync(genre);

        // Act
        await _genreService.DeleteGenreAsync(genre.Id.ToString());

        // Assert
        _unitOfWorkMock.Verify(u => u.Genres.Delete(genre), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteGenreAsync_InvalidIdFormat_ThrowsArgumentException()
    {
        // Act/Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _genreService.DeleteGenreAsync("1-1-1"));
    }

    [Fact]
    public async Task DeleteGenreAsync_InvalidGuidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        string id = "11111111-1111-1111-1111-111111111111";
        var guidId = Guid.Parse(id);
        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(guidId, false))
            .ReturnsAsync((Genre)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _genreService.DeleteGenreAsync(id));
    }

    [Fact]
    public async Task DeleteGenreAsync_InvalidIntId_ThrowsEntityNotFoundException()
    {
        // Arrange
        string id = "1";
        _categoryRepositoryMock.Setup(c => c.DeleteByIdAsync(int.Parse(id))).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _genreService.DeleteGenreAsync(id));
    }

    [Fact]
    public async Task UpdateGenreAsync_ValidData_ShouldUpdateGenre()
    {
        // Arrange
        var updateGenreDto = GenreServiceTestData.UpdateGenreDto;

        var existingGenre = new Genre
        {
            Id = Guid.Parse(updateGenreDto.Id),
            Name = updateGenreDto.Name,
        };

        _unitOfWorkMock.Setup(u => u.Genres.NameExist(updateGenreDto.Name)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(Guid.Parse(updateGenreDto.ParentGenreId!), true))
            .ReturnsAsync(new Genre { Id = Guid.Parse(updateGenreDto.ParentGenreId!) });
        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(existingGenre.Id, false))
            .ReturnsAsync(existingGenre);

        // Act
        await _genreService.UpdateGenreAsync(updateGenreDto);

        // Assert
        _unitOfWorkMock.Verify(u => u.Genres.Update(It.IsAny<Genre>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateGenreAsync_ValidCategoryData_ShouldCopyCategory()
    {
        // Arrange
        var updateGenreDto = GenreServiceTestData.UpdateProductDto;
        var categoryId = int.Parse(updateGenreDto.Id);

        _categoryRepositoryMock.Setup(c => c.GetByIdAsync(categoryId, false))
            .ReturnsAsync(new Category { CategoryID = categoryId });
        _unitOfWorkMock.Setup(u => u.Genres.NameExist(updateGenreDto.Name)).Returns(false);
        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(Guid.Parse(updateGenreDto.ParentGenreId!), true))
            .ReturnsAsync(new Genre { Id = Guid.Parse(updateGenreDto.ParentGenreId!) });
        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(It.IsAny<Guid>(), true))
            .ReturnsAsync(new Genre { Name = "ParentGenre" });

        // Act
        await _genreService.UpdateGenreAsync(updateGenreDto);

        // Assert
        _unitOfWorkMock.Verify(u => u.Genres.AddAsync(It.IsAny<Genre>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGenreAsync_InvalidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var updateGenreDto = GenreServiceTestData.UpdateGenreDto;

        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(It.IsAny<Guid>(), false))
            .ReturnsAsync((Genre)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _genreService.UpdateGenreAsync(updateGenreDto));
    }

    [Fact]
    public async Task UpdateGenreAsync_InvalidParentGenreId_ThrowsIdsNotValidException()
    {
        // Arrange
        var updateGenreDto = GenreServiceTestData.UpdateGenreDto;
        var guidId = Guid.Parse(updateGenreDto.Id);

        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(guidId, false))
            .ReturnsAsync(new Genre { Id = guidId, Name = updateGenreDto.Name });
        _unitOfWorkMock.Setup(u => u.Genres.NameExist(updateGenreDto.Name)).Returns(true);
        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(Guid.Parse(updateGenreDto.ParentGenreId!), false))
            .ReturnsAsync((Genre)null);

        // Act & Assert
        await Assert.ThrowsAsync<IdsNotValidException>(() => _genreService.UpdateGenreAsync(updateGenreDto));
    }

    [Fact]
    public async Task UpdateGenreAsync_InvalidGenreName_ThrowsUniquePropertyException()
    {
        // Arrange
        var updateGenreDto = GenreServiceTestData.UpdateGenreDto;
        var guidId = Guid.Parse(updateGenreDto.Id);

        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(guidId, false))
            .ReturnsAsync(new Genre { Id = guidId, Name = "Another name" });
        _unitOfWorkMock.Setup(u => u.Genres.NameExist(updateGenreDto.Name)).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<UniquePropertyException>(() => _genreService.UpdateGenreAsync(updateGenreDto));
    }

    [Fact]
    public async Task GetAllGenresAsync_ReturnsListOfGenres()
    {
        // Arrange
        var genres = new List<Genre>
        {
            new() { Name = "Test genre" },
            new() { Name = "Test genre 2 " },
        };

        _unitOfWorkMock.Setup(u => u.Genres.GetAllAsync(false))
            .ReturnsAsync(genres);

        // Act
        var result = await _genreService.GetAllGenresAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetGenreByIdAsync_InvalidGuidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(It.IsAny<Guid>(), false))
            .ReturnsAsync((Genre)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
        _genreService.GetGenreByIdAsync("11111111-1111-1111-1111-111111111111"));
    }

    [Fact]
    public async Task GetGenreByIdAsync_InvalidIntId_ThrowsEntityNotFoundException()
    {
        // Arrange
        _categoryRepositoryMock.Setup(c => c.GetByIdAsync(It.IsAny<int>(), false)).ReturnsAsync((Category)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
        _genreService.GetGenreByIdAsync("1"));
    }

    [Fact]
    public async Task GetGenreByIdAsync_ValidGuidId_ReturnsGenre()
    {
        // Arrange
        var validId = GenreServiceTestData.GenreEntity.Id;
        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(validId, false))
            .ReturnsAsync(GenreServiceTestData.GenreEntity);

        // Act
        var result = await _genreService.GetGenreByIdAsync(validId.ToString());

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetGenreByIdAsync_ValidIntId_ReturnsCategory()
    {
        // Arrange
        var validId = GenreServiceTestData.Category.CategoryID;
        _categoryRepositoryMock.Setup(c => c.GetByIdAsync(validId, false)).ReturnsAsync(GenreServiceTestData.Category);

        // Act
        var result = await _genreService.GetGenreByIdAsync(validId.ToString());

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetGenreByIdAsync_InvalidIdFormat_ThrowsArgumentException()
    {
        // Act/Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _genreService.GetGenreByIdAsync("1-1-1"));
    }

    [Fact]
    public async Task GetGenresByParentIdAsync_InvalidId_ThrowsEntityNotFoundException()
    {
        // Arrange
        List<Genre> genreList = null;
        _unitOfWorkMock.Setup(u => u.Genres.GetByParentIdAsync(It.IsAny<Guid>(), false))
            .ReturnsAsync(genreList!);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
        _genreService.GetGenresByParentIdAsync("11111111-1111-1111-1111-111111111111"));
    }

    [Fact]
    public async Task GetGenresByParentIdAsync_ValidId_ReturnsGenreList()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(It.IsAny<Guid>(), false))
            .ReturnsAsync(new Genre() { Name = "Parent Gernre", });
        _unitOfWorkMock.Setup(u => u.Genres.GetByParentIdAsync(It.IsAny<Guid>(), false))
            .ReturnsAsync(GenreServiceTestData.GenreList);

        // Act
        var result = await _genreService
            .GetGenresByParentIdAsync("11111111-1111-1111-1111-111111111111");

        // Assert
        Assert.NotEmpty(result);
    }
}
