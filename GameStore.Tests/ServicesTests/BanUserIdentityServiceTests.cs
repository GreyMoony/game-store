using GameStore.Application.Services.UserServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.ServicesTestData;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace GameStore.Tests.ServicesTests;
public class BanUserIdentityServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly BanUserIdentityService _service;

    public BanUserIdentityServiceTests()
    {
        _userManagerMock = MockUserManager.CreateUserManager<ApplicationUser>();
        _service = new BanUserIdentityService(_userManagerMock.Object);
    }

    [Fact]
    public async Task AddBannedUserAsync_WhenUserAlreadyBanned_ShouldExtendBanDuration()
    {
        // Arrange
        var user = BanServiceTestData.BannedUser;
        var banDto = BanServiceTestData.WeekBanDto;

        _userManagerMock.Setup(u => u.FindByNameAsync(banDto.User))
            .ReturnsAsync(user);
        _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.AddBannedUserAsync(banDto);

        // Assert
        Assert.NotNull(user.BanEndTime);
        _userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task AddBannedUserAsync_WhenUserNotBanned_ShouldBan()
    {
        // Arrange
        var user = BanServiceTestData.NotBannedUser;
        var banDto = BanServiceTestData.WeekBanDto;

        _userManagerMock.Setup(u => u.FindByNameAsync(banDto.User))
            .ReturnsAsync(user);
        _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.AddBannedUserAsync(banDto);

        // Assert
        Assert.NotNull(user.BanEndTime);
        _userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task AddBannedUserAsync_WhenUserNotBanned_ShouldBanPermanent()
    {
        // Arrange
        var user = BanServiceTestData.NotBannedUser;
        var banDto = BanServiceTestData.PermanentBanDto;

        _userManagerMock.Setup(u => u.FindByNameAsync(banDto.User))
            .ReturnsAsync(user);
        _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.AddBannedUserAsync(banDto);

        // Assert
        Assert.True(user.IsBannedFromCommenting);
        Assert.Null(user.BanEndTime);
        _userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task AddBannedUserAsync_WhenUserNotFound_ShouldThrowException()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _service.AddBannedUserAsync(BanServiceTestData.WeekBanDto));
    }

    [Fact]
    public async Task IsUserBannedAsync_WhenBanExpired_ShouldReturnFalse()
    {
        // Arrange
        var user = BanServiceTestData.ExpiredBanUser;
        _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _userManagerMock.Setup(u => u.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.IsUserBannedAsync(user.UserName!);

        // Assert
        Assert.False(result);
        Assert.False(user.IsBannedFromCommenting);
        _userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task IsUserBannedAsync_WhenBanNotExpired_ShouldReturnTrue()
    {
        // Arrange
        var user = BanServiceTestData.BannedUser;
        _userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.IsUserBannedAsync(user.UserName!);

        // Assert
        Assert.True(result);
    }
}
