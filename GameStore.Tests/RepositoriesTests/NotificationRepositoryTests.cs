using GameStore.DAL.Data;
using GameStore.DAL.Repositories;
using GameStore.Domain.Enums;
using GameStore.Tests.TestUtilities;

namespace GameStore.Tests.RepositoriesTests;
public class NotificationRepositoryTests
{
    private readonly AuthDbContext _context;
    private readonly NotificationsRepository _repository;
    private readonly AuthTestData _testData;

    public NotificationRepositoryTests()
    {
        _context = GameStoreContextFactory.CreateInMemoryAuthContext();
        _testData = TestDataSeeder.SeedAuthDatabase(_context);
        _repository = new NotificationsRepository(_context);
    }

    ~NotificationRepositoryTests()
    {
        _context.Database.EnsureDeletedAsync();
        _context.Dispose();
    }

    [Fact]
    public async Task GetUserMethodsAsync_ValidUserId_ShouldReturnNotificationList()
    {
        // Arrange
        var userId = _testData.Users[0].Id;
        var expectedMethods = _testData.Methods
            .Where(m => m.UserId == userId)
            .Select(m => m.Method.ToString().ToLowerInvariant())
            .ToList();

        // Act
        var result = await _repository.GetUserMethodsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMethods, result);
    }

    [Fact]
    public async Task UpdateUserMethodsAsync_ValidUserId_ShouldUpdateMethods()
    {
        // Arrange
        var userId = _testData.Users[0].Id;
        var newMethods = new List<NotificationMethodType>
        {
            NotificationMethodType.Email,
            NotificationMethodType.Push,
        };

        // Act
        await _repository.UpdateUserMethodsAsync(userId, newMethods);

        // Assert
        var updatedMethods = _context.UserNotificationMethods.Where(m => m.UserId == userId)
            .ToList();
        Assert.Equal(newMethods.Count, updatedMethods.Count);
    }
}
