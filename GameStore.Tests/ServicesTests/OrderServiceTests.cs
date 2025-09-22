using AutoMapper;
using GameStore.API.Mappings;
using GameStore.Application.DTOs.OrderDtos;
using GameStore.Application.Helpers;
using GameStore.Application.Services.NotificationServices;
using GameStore.Application.Services.OrderServices;
using GameStore.Application.Services.PaymentStrategies;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Enums;
using GameStore.Domain.Exceptions;
using GameStore.Domain.Settings;
using GameStore.Tests.TestUtilities.ServicesTestData;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace GameStore.Tests.ServicesTests;
public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkMongo> _unitOfWorkMockMongo;
    private readonly Mock<IPaymentStrategyFactory> _paymentStrategyFactoryMock;
    private readonly Mock<IPaymentStrategy> _paymentStrategyMock;
    private readonly Mock<IOptions<InvoiceSettings>> _optionsMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMockMongo = new Mock<IUnitOfWorkMongo>();
        _paymentStrategyMock = new Mock<IPaymentStrategy>();
        _paymentStrategyFactoryMock = new Mock<IPaymentStrategyFactory>();
        _paymentStrategyFactoryMock
            .Setup(f => f.GetStrategy(It.IsAny<string>())).Returns(_paymentStrategyMock.Object);

        _optionsMock = new Mock<IOptions<InvoiceSettings>>();
        _optionsMock.Setup(o => o.Value).Returns(new InvoiceSettings { ValidityDays = 30 });

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();

        var mockLogger = new Mock<ILogger<EntityChangeLogger>>();
        var changeLogger = new EntityChangeLogger(mockLogger.Object);
        var notificationServiceMock = new Mock<INotificationService>();

        _orderService = new OrderService(
            _unitOfWorkMock.Object,
            _unitOfWorkMockMongo.Object,
            _paymentStrategyFactoryMock.Object,
            mapper,
            changeLogger,
            notificationServiceMock.Object);
    }

    [Fact]
    public async Task AddGameToCartAsync_InvalidGameKey_ThrowsEntityNotFoundException()
    {
        // Arrange
        string gameKey = "Game not exist";

        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(gameKey, false)).ReturnsAsync((Game)null);
        _unitOfWorkMockMongo.Setup(u => u.Products.GetByKeyAsync(gameKey, false)).ReturnsAsync((Product)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _orderService.AddGameToCartAsync(Guid.NewGuid(), gameKey));
    }

    [Fact]
    public async Task AddGameToCartAsync_QuantityMoreThenInStock_ThrowsInvalidOperationException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var game = OrderServiceTestData.GameFromCart;
        var gameKey = game.Key;
        int gameQuantity = game.UnitInStock + 1;

        _unitOfWorkMock.Setup(uow => uow.Games.GetByKeyAsync(gameKey, false))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(uow => uow.Orders.GetCartAsync(customerId))
            .ReturnsAsync((Order)null);

        // Act
        await _orderService.AddGameToCartAsync(Guid.NewGuid(), gameKey, gameQuantity);

        // Assert
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task AddGameToCartAsync_WhenGameAlreadyInCart_ShouldAddGameToCart()
    {
        // Arrange
        var cart = OrderServiceTestData.CartWithOneGame;
        var customerId = cart.CustomerId;
        var game = OrderServiceTestData.GameFromCart;
        var gameKey = game.Key;
        int quantityToAdd = 2;
        int totalQuantity = quantityToAdd +
            cart.OrderGames.FirstOrDefault(og => og.ProductId == game.Id).Quantity;

        _unitOfWorkMock.Setup(uow => uow.Games.GetByKeyAsync(gameKey, false))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(uow => uow.Games.GetByIdAsync(game.Id, false))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(uow => uow.Orders.GetCartAsync(customerId))
            .ReturnsAsync(cart);

        // Act
        await _orderService.AddGameToCartAsync(customerId, gameKey, quantityToAdd);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.Orders.Update(It.Is<Order>(
                o =>
                o.OrderGames.Any(
                    og =>
                    og.ProductId == game.Id &&
                    og.Quantity == totalQuantity &&
                    Math.Abs(og.Price - game.Price) < 0.0001 &&
                    og.Discount == game.Discount))),
            Times.Once);

        _unitOfWorkMock.Verify(uow => uow.Orders.Update(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task AddGameToCartAsync_WhenCartIsEmpty_ShouldAddGameToCart()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var game = OrderServiceTestData.GameFromCart;
        var gameKey = game.Key;
        int quantityToAdd = 2;

        _unitOfWorkMock.Setup(uow => uow.Games.GetByKeyAsync(gameKey, false))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(uow => uow.Games.GetByIdAsync(game.Id, false))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(uow => uow.Orders.GetCartAsync(customerId))
            .ReturnsAsync((Order)null);

        // Act
        await _orderService.AddGameToCartAsync(customerId, gameKey, quantityToAdd);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.Orders.Update(It.Is<Order>(
                o =>
                o.OrderGames.Any(
                    og =>
                    og.ProductId == game.Id &&
                    og.Quantity == quantityToAdd &&
                    Math.Abs(og.Price - game.Price) < 0.0001 &&
                    og.Discount == game.Discount))),
            Times.Once);
    }

    [Fact]
    public async Task GetCartAsync_CartExist_ReturnsCart()
    {
        // Arrange
        var cart = OrderServiceTestData.CartWithOneGame;
        _unitOfWorkMock.Setup(u => u.Orders.GetCartAsync(cart.CustomerId)).ReturnsAsync(cart);

        // Act
        var result = await _orderService.GetCartAsync(cart.CustomerId);

        // Assert
        Assert.Equal(cart.Id, result.OrderId);
        Assert.Equal(cart.OrderGames.Count, result.Details.Count());
    }

    [Fact]
    public async Task GetCartAsync_CartNotExist_ReturnsEmptyCart()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Orders.GetCartAsync(It.IsAny<Guid>())).ReturnsAsync((Order)null);

        // Act
        var result = await _orderService.GetCartAsync(Guid.NewGuid());

        // Assert
        Assert.Empty(result.Details);
    }

    [Fact]
    public async Task GetOrderById_OrderNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Orders.GetByIdAsync(It.IsAny<Guid>(), false)).ReturnsAsync((Order)null);

        // Act/Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _orderService.GetOrderById("11111111-1111-1111-1111-111111111111"));
    }

    [Fact]
    public async Task GetOrderById_ValidId_ReturnsOrder()
    {
        // Arrange
        var order = OrderServiceTestData.Order;
        _unitOfWorkMock.Setup(u => u.Orders.GetByIdAsync(order.Id, false)).ReturnsAsync(order);

        // Act
        var result = await _orderService.GetOrderById(order.Id.ToString());

        // Assert
        Assert.Equal(order.Id.ToString(), result.Id);
        Assert.Equal(order.CustomerId.ToString(), result.CustomerId);
    }

    [Fact]
    public async Task GetOrderDetails_ReturnsOrderDetailsList()
    {
        // Arrange
        var order = OrderServiceTestData.Order;
        var orderDetails = order.OrderGames;
        _unitOfWorkMock.Setup(u => u.Orders.GetOrderDetailsAsync(order.Id)).ReturnsAsync(orderDetails);

        // Act
        var result = await _orderService.GetOrderDetails(order.Id.ToString());

        // Assert
        Assert.Equal(orderDetails.Count, result.Count());
    }

    [Fact]
    public async Task GetPaidAndCancelledOrders_ReturnsOrderDtosList()
    {
        // Arrange
        var orders = OrderServiceTestData.PaidAndCanceledOrders;
        _unitOfWorkMock.Setup(u => u.Orders.GetPaidAndCancelledOrdersAsync()).ReturnsAsync(orders);

        // Act
        var result = await _orderService.GetPaidAndCancelledOrders();

        // Assert
        Assert.Equal(orders.Count(), result.Count());
    }

    [Fact]
    public void GetPaymentMethods_ReturnsPaymentMethodsList()
    {
        // Act
        var result = _orderService.GetPaymentMethods();

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Contains(result, pm => pm.Title
        is "Visa" or "Bank" or "IBox terminal");
    }

    [Fact]
    public async Task RemoveGameFromCartAsync_InvalidGameKey_ThrowsEntityNotFoundException()
    {
        // Arrange
        string gameKey = "Game not exist";

        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(gameKey, false)).ReturnsAsync((Game)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _orderService.RemoveGameFromCartAsync(Guid.NewGuid(), gameKey));
    }

    [Fact]
    public async Task RemoveGameFromCartAsync_CartNNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        string gameKey = "Game not exist";
        var customerId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(gameKey, false))
            .ReturnsAsync(new Game() { Id = Guid.NewGuid() });
        _unitOfWorkMock.Setup(u => u.Orders.GetCartAsync(customerId))
            .ReturnsAsync((Order)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _orderService.RemoveGameFromCartAsync(customerId, gameKey));
    }

    [Fact]
    public async Task RemoveGameFromCartAsync_TheOnlyGameInCart_ShouldDeleteCart()
    {
        // Arrange
        var cart = OrderServiceTestData.CartWithOneGame;
        var customerId = cart.CustomerId;
        var game = OrderServiceTestData.GameFromCart;
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(game.Key, false))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(uow => uow.Games.GetByIdAsync(game.Id, false))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(u => u.Orders.GetCartAsync(customerId))
            .ReturnsAsync(cart);

        // Act
        await _orderService.RemoveGameFromCartAsync(customerId, game.Key);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.Orders.Delete(It.Is<Order>(
                o => o.Id == cart.Id)),
            Times.Once);
    }

    [Fact]
    public async Task RemoveGameFromCartAsync_NotOnlyGameInCart_ShouldDeleteGameFromCart()
    {
        // Arrange
        var cart = OrderServiceTestData.CartWithTwoGames;
        var customerId = cart.CustomerId;
        var game = OrderServiceTestData.GameFromCart;
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(game.Key, false))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(uow => uow.Games.GetByIdAsync(game.Id, false))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(u => u.Orders.GetCartAsync(customerId))
            .ReturnsAsync(cart);

        // Act
        await _orderService.RemoveGameFromCartAsync(customerId, game.Key);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.Orders.Update(It.Is<Order>(
                o =>
                o.OrderGames.All(
                    og =>
                    og.ProductId != game.Id))),
            Times.Once);
    }

    [Fact]
    public async Task ProcessOrderPaymentAsync_CartNotExist_ThrowEntityNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Orders.GetCartAsync(It.IsAny<Guid>())).ReturnsAsync((Order)null);

        // Act/Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _orderService.ProcessOrderPaymentAsync(Guid.NewGuid(), It.IsAny<PaymentRequestDto>()));
    }

    [Fact]
    public async Task ProcessOrderPaymentAsync_VisaMethodSuccessPayment_ReturnsSuccess()
    {
        // Arrange
        var userId = OrderServiceTestData.CartWithOneGame.CustomerId;
        var cart = OrderServiceTestData.CartWithOneGame;
        var game = OrderServiceTestData.GameFromCart;
        var paymentRequest = new PaymentRequestDto()
        {
            Method = "Visa",
            Model = new() { Holder = "Payer Name" },
        };
        _unitOfWorkMock.Setup(u => u.Orders.GetCartAsync(userId)).ReturnsAsync(cart);
        _unitOfWorkMock.Setup(u => u.Games.GetByIdAsync(game.Id, false)).ReturnsAsync(game);
        _paymentStrategyMock
            .Setup(vs => vs.ProcessPaymentAsync(cart, paymentRequest))
            .ReturnsAsync(new PaymentResult { ResultType = PaymentResultType.PaymentSuccess });

        // Act
        var result = await _orderService.ProcessOrderPaymentAsync(userId, paymentRequest);

        // Assert
        Assert.Equal(PaymentResultType.PaymentSuccess, result.ResultType);
    }

    [Fact]
    public async Task ProcessOrderPaymentAsync_IBoxMethodSuccessPayment_ReturnsSuccess()
    {
        // Arrange
        var userId = OrderServiceTestData.CartWithOneGame.CustomerId;
        var cart = OrderServiceTestData.CartWithOneGame;
        var game = OrderServiceTestData.GameFromCart;
        var paymentRequest = new PaymentRequestDto()
        {
            Method = "IBox terminal",
        };
        _unitOfWorkMock.Setup(u => u.Orders.GetCartAsync(userId)).ReturnsAsync(cart);
        _unitOfWorkMock.Setup(u => u.Games.GetByIdAsync(game.Id, false)).ReturnsAsync(game);
        _paymentStrategyMock
            .Setup(vs => vs.ProcessPaymentAsync(cart, paymentRequest))
            .ReturnsAsync(new PaymentResult { ResultType = PaymentResultType.PaymentSuccess });

        // Act
        var result = await _orderService.ProcessOrderPaymentAsync(userId, paymentRequest);

        // Assert
        Assert.Equal(PaymentResultType.PaymentSuccess, result.ResultType);
    }

    [Fact]
    public async Task ProcessOrderPaymentAsync_BoxStrategyFailedPayment_ReturnsFalseSuccess()
    {
        // Arrange
        var userId = OrderServiceTestData.CartWithOneGame.CustomerId;
        var cart = OrderServiceTestData.CartWithOneGame;
        var game = OrderServiceTestData.GameFromCart;
        var paymentRequest = new PaymentRequestDto()
        {
            Method = "IBox terminal",
        };
        _unitOfWorkMock.Setup(u => u.Orders.GetCartAsync(userId)).ReturnsAsync(cart);
        _unitOfWorkMock.Setup(u => u.Games.GetByIdAsync(game.Id, false)).ReturnsAsync(game);
        _paymentStrategyMock
            .Setup(vs => vs.ProcessPaymentAsync(cart, paymentRequest))
            .ReturnsAsync(new PaymentResult { ResultType = PaymentResultType.PaymentFailure });

        // Act
        var result = await _orderService.ProcessOrderPaymentAsync(userId, paymentRequest);

        // Assert
        Assert.Equal(PaymentResultType.PaymentFailure, result.ResultType);
    }
}
