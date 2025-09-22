using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories;
public class OrderRepository(GameStoreContext context) :
    Repository<Order>(context), IOrderRepository
{
    public new async Task<Order?> GetByIdAsync(Guid id, bool includeDeleted = false)
    {
        var query = Context.Orders.Where(o => o.Id == id)
            .Include(o => o.OrderGames).ThenInclude(og => og.Product);

        var order = await query.FirstOrDefaultAsync(o => o.Id == id);

        return order;
    }

    public async Task<IEnumerable<Order>> GetByDateTimeAsync(DateTime? start, DateTime? end)
    {
        var query = Context.Orders
            .IgnoreQueryFilters();
        if (start.HasValue)
        {
            query = query.Where(o => o.Date >= start);
        }

        if (end.HasValue)
        {
            query = query.Where(o => o.Date <= end);
        }

        return await query.Include(o => o.OrderGames).ToListAsync();
    }

    public async Task<Order?> GetCartAsync(Guid customerId)
    {
        return await Context.Orders
            .Where(o => o.CustomerId == customerId && o.Status == OrderStatus.Open)
            .Include(o => o.OrderGames).ThenInclude(og => og.Product)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<OrderGame>> GetOrderDetailsAsync(Guid orderId)
    {
        return await Context.OrderGames.IgnoreQueryFilters()
            .Where(og => og.OrderId == orderId)
            .ToListAsync();
    }

    public async Task<OrderGame?> GetOrderDetailByIdAsync(Guid id)
    {
        return await Context.OrderGames.IgnoreQueryFilters()
            .FirstOrDefaultAsync(og => og.Id == id);
    }

    public async Task<IEnumerable<Order>> GetPaidAndCancelledOrdersAsync()
    {
        return await Context.Orders.Where(o =>
            o.Status == OrderStatus.Paid ||
            o.Status == OrderStatus.Cancelled).ToListAsync();
    }

    public void UpdateOrderGame(OrderGame orderGame)
    {
        Context.Entry(orderGame).State = EntityState.Modified;
    }

    public void DeleteOrderGame(OrderGame orderGame)
    {
        Context.Set<OrderGame>().Remove(orderGame);
    }

    public async Task AddOrderGameAsync(OrderGame orderGame)
    {
        await Context.AddAsync(orderGame);
    }
}
