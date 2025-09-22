using GameStore.Application.Services.NotificationServices;
using GameStore.Application.Services.NotificationServices.NotificationStrategies;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;
using GameStore.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace GameStore.Tests.ServicesTests;
public class NotificationServiceTests
{
    private readonly Mock<INotificationsRepository> _notificationsRepoMock = new();
    private readonly Mock<INotificationSender> _senderMock = new();
    private readonly Mock<INotificationSenderFactory> _senderFactoryMock = new();
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly NotificationService _notificationService;

    public NotificationServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _senderMock.Setup(s => s.SendAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<Order>())).Returns(Task.CompletedTask).Verifiable();
        _senderFactoryMock.Setup(f => f.GetSender(NotificationMethodType.Email)).Returns(_senderMock.Object);

        _notificationService = new NotificationService(
            _notificationsRepoMock.Object, _senderFactoryMock.Object, _userManagerMock.Object);
    }

    [Fact]
    public async Task GetUserNotificationMethodsAsync_ReturnsMethods()
    {
        // Arrange
        var userId = "user1";
        var methods = new List<string> { "Email", "Sms" };
        _notificationsRepoMock.Setup(r => r.GetUserMethodsAsync(userId)).ReturnsAsync(methods);

        // Act
        var result = await _notificationService.GetUserNotificationMethodsAsync(userId);

        // Assert
        Assert.Equal(methods, result);
    }

    [Fact]
    public async Task UpdateUserNotificationMethodsAsync_CallsRepositoryUpdateMethod()
    {
        // Arrange
        var userId = "user1";
        var methods = new List<NotificationMethodType> { NotificationMethodType.Email };
        _notificationsRepoMock.Setup(r => r.UpdateUserMethodsAsync(userId, methods)).Returns(Task.CompletedTask).Verifiable();

        // Act
        await _notificationService.UpdateUserNotificationMethodsAsync(userId, methods);

        // Assert
        _notificationsRepoMock.Verify(r => r.UpdateUserMethodsAsync(userId, methods), Times.Once);
    }

    [Fact]
    public async Task NotifyOrderStatusChangedAsync_SendsNotifications()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Checkout,
            OrderGames =
                [
                    new OrderGame
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 1,
                    },
                ],
        };
        var user = new ApplicationUser
        {
            Id = order.CustomerId.ToString(),
            Email = "test@email.com",
        };
        var manager = new ApplicationUser
        {
            Id = "manager1",
            Email = "manager@email.com",
        };
        _userManagerMock.Setup(m => m.FindByIdAsync(order.CustomerId.ToString()))
            .ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetUsersInRoleAsync(UserRoles.Manager))
            .ReturnsAsync([manager]);
        _notificationsRepoMock.Setup(r => r.GetUserMethodsAsync(user.Id))
            .ReturnsAsync(["Email"]);
        _notificationsRepoMock.Setup(r => r.GetUserMethodsAsync(manager.Id))
            .ReturnsAsync(["Email"]);

        // Act
        await _notificationService.NotifyOrderStatusChangedAsync(order);

        // Assert
        _senderMock.Verify(s => s.SendAsync(user, It.IsAny<string>(), order), Times.Once);
        _senderMock.Verify(s => s.SendAsync(manager, It.IsAny<string>(), order), Times.Once);
    }

    [Fact]
    public async Task NotifyOrderStatusChangedAsync_UserIdFromOrderNotValid_ThrowsEntityNotFoundException()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Checkout,
            OrderGames =
                [
                    new OrderGame
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 1,
                    },
                ],
        };
        _userManagerMock.Setup(m => m.FindByIdAsync(order.CustomerId.ToString()))
            .ReturnsAsync((ApplicationUser)null);

        // Act/Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _notificationService.NotifyOrderStatusChangedAsync(order));
    }
}
