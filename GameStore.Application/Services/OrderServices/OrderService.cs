using AutoMapper;
using GameStore.Application.DTOs.OrderDtos;
using GameStore.Application.Helpers;
using GameStore.Application.Services.NotificationServices;
using GameStore.Application.Services.PaymentStrategies;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;
using GameStore.Domain.Exceptions;

namespace GameStore.Application.Services.OrderServices;
#pragma warning disable SA1010, IDE0305 // Opening square brackets should be spaced correctly
public class OrderService(
    IUnitOfWork unitOfWork,
    IUnitOfWorkMongo unitOfWorkMongo,
    IPaymentStrategyFactory strategyFactory,
    IMapper mapper,
    EntityChangeLogger changeLogger,
    INotificationService notificationService) : IOrderService
{
    public async Task AddGameToCartAsync(Guid customerId, string gameKey, int quantity = 1)
    {
        Game game = await GetGameByKey(gameKey);

        if (quantity > game.UnitInStock)
        {
            return;
        }

        var cart = await unitOfWork.Orders.GetCartAsync(customerId);

        if (cart == null)
        {
            var orderGames = new List<OrderGame>();
            cart = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Status = OrderStatus.Open,
                Date = DateTime.UtcNow,
                OrderGames = orderGames,
            };
            await unitOfWork.Orders.AddAsync(cart);
            await unitOfWork.SaveAsync();
        }

        var orderGame = cart.OrderGames.FirstOrDefault(og => og.ProductId == game.Id);

        if (orderGame != null)
        {
            orderGame.Quantity += quantity;
        }
        else
        {
            cart.OrderGames.Add(new OrderGame
            {
                OrderId = cart.Id,
                ProductId = game.Id,
                Price = game.Price,
                Discount = game.Discount,
                Quantity = quantity,
            });
        }

        await TakeFromStockAsync(game.Id, quantity);
        unitOfWork.Orders.Update(cart);
        await unitOfWork.SaveAsync();
    }

    public async Task UpdateOrderDetailQuantityAsync(Guid id, int count)
    {
        var orderGame = await unitOfWork.Orders.GetOrderDetailByIdAsync(id) ??
            throw new EntityNotFoundException($"There is no order detail with id {id}");

        var game = await unitOfWork.Games.GetByIdAsync(orderGame.ProductId) ??
            throw new EntityNotFoundException($"There is no game with id {orderGame.ProductId}");

        var dif = count - orderGame.Quantity;

        if (dif > game.UnitInStock || dif == 0)
        {
            return;
        }

        orderGame.Quantity = count;
        unitOfWork.Orders.UpdateOrderGame(orderGame);
        await unitOfWork.SaveAsync();

        if (dif > 0)
        {
            await TakeFromStockAsync(game.Id, dif);
        }
        else
        {
            await ReturnToStockAsync(game.Id, dif);
        }
    }

    public async Task DeleteOrderDetailAsync(Guid id)
    {
        var orderGame = await unitOfWork.Orders.GetOrderDetailByIdAsync(id) ??
           throw new EntityNotFoundException($"There is no order detail with id {id}");

        var quantityToReturn = orderGame.Quantity;

        unitOfWork.Orders.DeleteOrderGame(orderGame);
        await unitOfWork.SaveAsync();

        await ReturnToStockAsync(orderGame.ProductId, quantityToReturn);
    }

    public async Task<Cart> GetCartAsync(Guid customerId)
    {
        var cart = await unitOfWork.Orders.GetCartAsync(customerId);

        return cart == null
            ? new Cart
            {
                OrderId = Guid.Empty,
                Details = [],
            }
            : new Cart
            {
                OrderId = cart.Id,
                Details = mapper.Map<IEnumerable<OrderDetailsDto>>(cart.OrderGames),
            };
    }

    public async Task<OrderDto> GetOrderById(string id)
    {
        if (Guid.TryParse(id, out var guidId))
        {
            var order = await unitOfWork.Orders.GetByIdAsync(guidId) ??
            throw new EntityNotFoundException($"Order with id {id} not exist");

            return mapper.Map<OrderDto>(order);
        }
        else if (int.TryParse(id, out var intId))
        {
            var mongoOrder = await unitOfWorkMongo.Orders.GetByIdAsync(intId) ??
            throw new EntityNotFoundException($"Order with id {id} not exist");

            return mapper.Map<OrderDto>(mongoOrder);
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided ID is not a valid Guid or ObjectId.", nameof(id));
        }
    }

    public async Task<IEnumerable<OrderDetailsDto>> GetOrderDetails(string id)
    {
        if (Guid.TryParse(id, out var guidId))
        {
            var details = await unitOfWork.Orders.GetOrderDetailsAsync(guidId);

            return mapper.Map<IEnumerable<OrderDetailsDto>>(details);
        }
        else if (int.TryParse(id, out var intId))
        {
            var orderDetails = await unitOfWorkMongo.Orders.GetOrderDetailsAsync(intId);

            return mapper.Map<IEnumerable<OrderDetailsDto>>(orderDetails);
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided ID is not a valid Guid or ObjectId.", nameof(id));
        }
    }

    public async Task<IEnumerable<OrderDto>> GetPaidAndCancelledOrders()
    {
        var orders = await unitOfWork.Orders.GetPaidAndCancelledOrdersAsync();

        return mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public IEnumerable<PaymentMethodDto> GetPaymentMethods()
    {
        return PaymentMethods.List;
    }

    public async Task<PaymentResult> ProcessOrderPaymentAsync(Guid userId, PaymentRequestDto request)
    {
        var cart = await unitOfWork.Orders.GetCartAsync(userId)
            ?? throw new EntityNotFoundException($"Cart was not found");

        cart.Status = OrderStatus.Checkout;
        unitOfWork.Orders.Update(cart);
        await unitOfWork.SaveAsync();

        var paymentStrategy = strategyFactory.GetStrategy(request.Method);
        var paymentResult = await paymentStrategy.ProcessPaymentAsync(cart, request);

        if (paymentResult.ResultType is PaymentResultType.PaymentSuccess)
        {
            cart.Status = OrderStatus.Paid;
            unitOfWork.Orders.Update(cart);
            await unitOfWork.SaveAsync();
        }
        else if (paymentResult.ResultType is PaymentResultType.PaymentFailure)
        {
            cart.Status = OrderStatus.Cancelled;
            unitOfWork.Orders.Update(cart);
            await unitOfWork.SaveAsync();
        }

        await notificationService.NotifyOrderStatusChangedAsync(cart);
        return paymentResult;
    }

    public async Task RemoveGameFromCartAsync(Guid customerId, string gameKey)
    {
        var game = await unitOfWork.Games.GetByKeyAsync(gameKey)
            ?? throw new EntityNotFoundException($"Game with key {gameKey} not exist");

        var cart = await unitOfWork.Orders.GetCartAsync(customerId)
            ?? throw new EntityNotFoundException($"Cart for customer {customerId} was not found");

        var orderGame = cart.OrderGames.FirstOrDefault(og => og.ProductId == game.Id);
        if (orderGame != null)
        {
            cart.OrderGames.Remove(orderGame);
            await ReturnToStockAsync(game.Id, orderGame.Quantity);

            if (cart.OrderGames.Count == 0)
            {
                unitOfWork.Orders.Delete(cart);
            }
            else
            {
                unitOfWork.Orders.Update(cart);
            }

            await unitOfWork.SaveAsync();
        }
    }

    public async Task CheckoutOrder(Guid customerId)
    {
        var cart = await unitOfWork.Orders.GetCartAsync(customerId);

        cart.Status = OrderStatus.Checkout;

        unitOfWork.Orders.Update(cart);
        await unitOfWork.SaveAsync();

        await notificationService.NotifyOrderStatusChangedAsync(cart);
    }

    public async Task<IEnumerable<OrderDto>> GetOrderHistory(DateTime? start, DateTime? end)
    {
        if (!start.HasValue && !end.HasValue)
        {
            end = DateTime.UtcNow.AddDays(-30);
        }

        var ordersFromSql = mapper.Map<IEnumerable<OrderDto>>(await unitOfWork.Orders.GetByDateTimeAsync(start, end));
        var ordersFromMongo = mapper.Map<IEnumerable<OrderDto>>(await unitOfWorkMongo.Orders.GetByDateTimeAsync(start, end));

        return ordersFromSql.Concat(ordersFromMongo).ToList();
    }

    public async Task ChangeStatusToShipped(Guid id)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(id) ??
            throw new EntityNotFoundException($"Order with id {id} was not found");

        if (order.Status == OrderStatus.Paid)
        {
            order.Status = OrderStatus.Shipped;
            unitOfWork.Orders.Update(order);
            await unitOfWork.SaveAsync();

            await notificationService.NotifyOrderStatusChangedAsync(order);
        }
    }

    public async Task AddGameToOrder(Guid orderId, string gameKey)
    {
        Game game = await GetGameByKey(gameKey);

        var order = await unitOfWork.Orders.GetByIdAsync(orderId) ??
            throw new EntityNotFoundException($"Order with id {orderId} was not found");

        if (order.OrderGames.Any(og => og.ProductId == game.Id) || game.UnitInStock <= 0)
        {
            return;
        }

        var orderGame = new OrderGame
        {
            Id = Guid.NewGuid(),
            ProductId = game.Id,
            OrderId = order.Id,
            Price = game.Price,
            Discount = game.Discount,
            Quantity = 1,
        };

        await unitOfWork.Orders.AddOrderGameAsync(orderGame);
        await unitOfWork.SaveAsync();
        await TakeFromStockAsync(game.Id, 1);
    }

    private async Task TakeFromStockAsync(Guid gameId, int quantity)
    {
        var game = await unitOfWork.Games.GetByIdAsync(gameId) ??
            throw new EntityNotFoundException($"Game with id {gameId} not found");
        game.UnitInStock -= quantity;
        unitOfWork.Games.Update(game);
        await unitOfWork.SaveAsync();
    }

    private async Task ReturnToStockAsync(Guid gameId, int quantity)
    {
        var game = await unitOfWork.Games.GetByIdAsync(gameId) ??
            throw new EntityNotFoundException($"Game with id {gameId} not found");
        game.UnitInStock += quantity;
        unitOfWork.Games.Update(game);
        await unitOfWork.SaveAsync();
    }

    private async Task<Game> GetGameByKey(string key)
    {
        Game game = await unitOfWork.Games.GetByKeyAsync(key);

        if (game is not null)
        {
            return game;
        }
        else
        {
            var product = await unitOfWorkMongo.Products.GetByKeyAsync(key)
                ?? throw new EntityNotFoundException($"Game with key {key} was not found");

            if (product.CopiedToSql.HasValue && product.CopiedToSql.Value)
            {
                game = await unitOfWork.Games.GetByProductIdAsync(product.ProductID!.Value)
                    ?? throw new EntityNotFoundException(
                        $"Game with ProductID {product.ProductID!.Value} was not found");
                return game;
            }
            else
            {
                game = mapper.Map<Game>(product);

                var genreId = await ProcessCategoryId(product.CategoryID!.Value);
                game.GameGenres =
                    [
                        new GameGenre
                        {
                            GameId = game.Id,
                            GenreId = genreId,
                        },
                    ];

                var publisherId = await ProcessSupplierId(product.SupplierID!.Value);
                game.PublisherId = publisherId;

                await unitOfWork.Games.AddAsync(game);
                await unitOfWork.SaveAsync();

                await unitOfWorkMongo.Products.MarkAsCopiedAsync(product.ProductID!.Value);

                changeLogger.LogNorthwindEntityChange(
                    "COPY", product.GetType().Name, product, game);
                return game;
            }
        }
    }

    private async Task<Guid> ProcessSupplierId(int supplierId)
    {
        var supplier = await unitOfWorkMongo.Suppliers.GetByIdAsync(supplierId);

        if (supplier.CopiedToSql.HasValue && supplier.CopiedToSql.Value)
        {
            var publisher = await unitOfWork.Publishers.GetBySupplierIdAsync(supplierId);
            return publisher.Id;
        }
        else
        {
            var publisher = mapper.Map<Publisher>(supplier);

            await unitOfWork.Publishers.AddAsync(publisher);
            await unitOfWork.SaveAsync();
            changeLogger.LogNorthwindEntityChange(
                "COPY", supplier.GetType().Name, supplier, publisher);
            await unitOfWorkMongo.Categories.MarkAsCopiedAsync(supplierId);

            return publisher.Id;
        }
    }

    private async Task<Guid> ProcessCategoryId(int categoryID)
    {
        var category = await unitOfWorkMongo.Categories.GetByIdAsync(categoryID);

        if (category.CopiedToSql.HasValue && category.CopiedToSql.Value)
        {
            var genre = await unitOfWork.Genres.GetByCategoryIdAsync(categoryID);
            return genre.Id;
        }
        else
        {
            var genre = mapper.Map<Genre>(category);

            await unitOfWork.Genres.AddAsync(genre);
            await unitOfWork.SaveAsync();
            changeLogger.LogNorthwindEntityChange(
                "COPY", category.GetType().Name, category, genre);
            await unitOfWorkMongo.Categories.MarkAsCopiedAsync(categoryID);

            return genre.Id;
        }
    }
}
