using GameStore.DAL.Data;
using GameStore.DAL.Repositories;
using GameStore.Tests.TestUtilities;

namespace GameStore.Tests.RepositoriesTests;
public class CommentRepositoryTests
{
    private readonly GameStoreContext _context;
    private readonly CommentRepository _repository;
    private readonly TestData _testData;

    public CommentRepositoryTests()
    {
        _context = GameStoreContextFactory.CreateInMemoryContext();
        _testData = TestDataSeeder.SeedDatabase(_context);
        _repository = new CommentRepository(_context);
    }

    ~CommentRepositoryTests()
    {
        _context.Database.EnsureDeletedAsync();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsComment()
    {
        // Arrange
        var id = _testData.Comments[0].Id;
        var actualComment = _testData.Comments[0];

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        Assert.Equal(actualComment, result);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnedCommentHasChildComment()
    {
        // Arrange
        var id = _testData.Comments.Find(c => c.ParentCommentId is not null).ParentCommentId!.Value;

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        Assert.NotEmpty(result.ChildComments);
    }

    [Fact]
    public async Task GetAllByGameKeyAsync_ReturnsListOfComments()
    {
        // Arrange
        var gameId = _testData.Comments[0].GameId;
        var key = _testData.Games.Find(g => g.Id == gameId).Key;

        // Act
        var result = await _repository.GetAllByGameKeyAsync(key);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.All(c => c.GameId == gameId));
    }
}
