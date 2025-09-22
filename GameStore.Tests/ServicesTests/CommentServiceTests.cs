using AutoMapper;
using GameStore.API.Mappings;
using GameStore.Application.DTOs.CommentDtos;
using GameStore.Application.Helpers;
using GameStore.Application.Services.CommentServices;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Exceptions;
using GameStore.Tests.TestUtilities.ServicesTestData;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameStore.Tests.ServicesTests;
public class CommentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkMongo> _unitOfWorkMockMongo;
    private readonly CommentService _commentService;

    public CommentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMockMongo = new Mock<IUnitOfWorkMongo>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        var mockLogger = new Mock<ILogger<EntityChangeLogger>>();
        var changeLogger = new EntityChangeLogger(mockLogger.Object);

        _commentService = new CommentService(
            _unitOfWorkMock.Object,
            _unitOfWorkMockMongo.Object,
            mapper,
            changeLogger);
    }

    [Fact]
    public async Task GetAllByGameKey_ReturnsListOfComments()
    {
        // Arrange
        var comments = CommentServiceTestData.CommentsList;
        var gameKey = "gameKey";
        _unitOfWorkMock.Setup(u => u.Comments.GetAllByGameKeyAsync(gameKey)).ReturnsAsync(comments);

        // Act
        var result = await _commentService.GetAllByGameKey(gameKey);

        // Assert
        Assert.Equal(comments.Count, result.Count());
    }

    [Fact]
    public async Task AddCommentAsync_InvalidGameKey_ThrowsException()
    {
        // Arrange
        var addCommentDto = new AddCommentDto
        {
            GameKey = "gameKey",
        };
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(addCommentDto.GameKey, false)).ReturnsAsync((Game)null);
        _unitOfWorkMockMongo.Setup(u => u.Products.GetByKeyAsync(addCommentDto.GameKey, false)).ReturnsAsync((Product)null);

        // Act/Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _commentService.AddCommentAsync(addCommentDto));
    }

    [Fact]
    public async Task AddCommentAsync_WithoutAction_AddsComment()
    {
        // Arrange
        var game = CommentServiceTestData.Game;
        var addCommentDto = new AddCommentDto
        {
            GameKey = game.Key,
            Name = "Test user",
            Body = "Comment text",
        };

        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(addCommentDto.GameKey, false)).ReturnsAsync(game);
        _unitOfWorkMock.Setup(u => u.Comments.AddAsync(It.IsAny<Comment>()))
        .Returns(Task.CompletedTask);

        // Act
        await _commentService.AddCommentAsync(addCommentDto);

        // Assert
        _unitOfWorkMock.Verify(
            u => u.Comments.AddAsync(
                It.Is<Comment>(
                    c => c.Body == addCommentDto.Body)),
            Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_GameFromMongo_AddsCommentWithCopiedProductsId()
    {
        // Arrange
        var product = CommentServiceTestData.Product;
        var game = CommentServiceTestData.Game;
        var addCommentDto = new AddCommentDto
        {
            GameKey = game.Key,
            Name = "Test user",
            Body = "Comment text",
        };

        _unitOfWorkMock.Setup(
            u => u.Games.GetByKeyAsync(addCommentDto.GameKey, false)).
            ReturnsAsync((Game)null);
        _unitOfWorkMockMongo.Setup(
            u => u.Products.GetByKeyAsync(addCommentDto.GameKey, false))
            .ReturnsAsync(product);
        _unitOfWorkMockMongo.Setup(
            u => u.Categories.GetByIdAsync(product.CategoryID!.Value, false))
            .ReturnsAsync(new Category { CategoryID = product.CategoryID!.Value });
        _unitOfWorkMockMongo.Setup(
            u => u.Suppliers.GetByIdAsync(product.SupplierID!.Value, false))
            .ReturnsAsync(new Supplier { SupplierID = product.SupplierID!.Value });
        _unitOfWorkMock.Setup(u => u.Genres.AddAsync(It.IsAny<Genre>()))
        .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Publishers.AddAsync(It.IsAny<Publisher>()))
        .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Games.AddAsync(It.IsAny<Game>()))
        .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.Comments.AddAsync(It.IsAny<Comment>()))
        .Returns(Task.CompletedTask);

        // Act
        await _commentService.AddCommentAsync(addCommentDto);

        // Assert
        _unitOfWorkMock.Verify(
            u => u.Genres.AddAsync(
                It.IsAny<Genre>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            u => u.Publishers.AddAsync(
                It.IsAny<Publisher>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            u => u.Games.AddAsync(
                It.IsAny<Game>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            u => u.Comments.AddAsync(
                It.Is<Comment>(
                    c => c.Body == addCommentDto.Body)),
            Times.Once);
    }

    [Theory]
    [MemberData(nameof(CommentServiceTestData.AddCommentTestData), MemberType = typeof(CommentServiceTestData))]
    public async Task AddCommentAsync_WithAction_AddsCommentWithExpectedBody(
        AddCommentDto addCommentDto, string expectedBody)
    {
        // Arrange
        var game = CommentServiceTestData.Game;
        var parentComment = CommentServiceTestData.ParentComment;

        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(addCommentDto.GameKey, false)).ReturnsAsync(game);
        _unitOfWorkMock.Setup(u => u.Comments.GetByIdAsync(addCommentDto.ParentId!.Value, false)).ReturnsAsync(parentComment);

        // Act
        await _commentService.AddCommentAsync(addCommentDto);

        // Assert
        _unitOfWorkMock.Verify(
            u => u.Comments.AddAsync(
                It.Is<Comment>(
                    c => c.Body == expectedBody)),
            Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_InvalidParentCommentId_ThrowsException()
    {
        // Arrange
        var game = CommentServiceTestData.Game;
        var addCommentDto = new AddCommentDto
        {
            GameKey = game.Key,
            Name = "Test user",
            Body = "Reply text",
            ParentId = Guid.NewGuid(),
            Action = CommentActions.Reply,
        };
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(It.IsAny<string>(), false))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(u => u.Comments.GetByIdAsync(It.IsAny<Guid>(), false))
            .ReturnsAsync((Comment)null);

        // Act/Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _commentService.AddCommentAsync(addCommentDto));
    }

    [Fact]
    public async Task DeleteCommentAsync_InvalidCommentId_ThrowsException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync("gameKey", false))
            .ReturnsAsync(new Game { Name = "Game1" });
        _unitOfWorkMock.Setup(u => u.Comments.GetByIdAsync(It.IsAny<Guid>(), false))
            .ReturnsAsync((Comment)null);

        // Act/Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _commentService.DeleteCommentAsync(It.IsAny<string>(), It.IsAny<Guid>(), false));
    }

    [Fact]
    public async Task DeleteCommentAsync_ValidId_ChangesCommentBody()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new Comment
        {
            Id = commentId,
            Name = "User name",
            Body = "Comment body",
        };
        var gameKey = "gameKey";

        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(gameKey, false))
            .ReturnsAsync(new Game { Name = "Game1" });
        _unitOfWorkMock.Setup(u => u.Comments.GetByIdAsync(commentId, false))
        .ReturnsAsync(comment);

        // Act
        await _commentService.DeleteCommentAsync(gameKey, commentId, false);

        // Assert
        _unitOfWorkMock.Verify(
            u => u.Comments.Update(
                It.Is<Comment>(
                    comment =>
                    comment.Body == CommentService.DeletedCommentBody)),
            Times.Once);
    }

    [Fact]
    public async Task DeleteCommentAsync_ValidId_ChangesChildCommentBody()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var childComments = new List<Comment>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Body = "Comment body, quote text",
                ParentCommentId = commentId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Body = "Comment text",
                ParentCommentId = commentId,
            },
        };
        var comment = new Comment
        {
            Id = commentId,
            Name = "User name",
            Body = "Comment body",
            ChildComments = childComments,
        };
        var gameKey = "gameKey";

        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(gameKey, false))
            .ReturnsAsync(new Game { Name = "Game1" });
        _unitOfWorkMock.Setup(u => u.Comments.GetByIdAsync(commentId, false))
        .ReturnsAsync(comment);

        // Act
        await _commentService.DeleteCommentAsync(gameKey, commentId, false);

        // Assert
        _unitOfWorkMock.Verify(
            u => u.Comments.Update(
                It.Is<Comment>(
                    comment =>
                    comment.ChildComments.First().Body.StartsWith(CommentService.DeletedCommentBody))),
            Times.Once);
    }
}
