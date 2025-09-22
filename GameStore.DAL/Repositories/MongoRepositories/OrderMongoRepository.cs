using System.Globalization;
using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameStore.DAL.Repositories.MongoRepositories;
public class OrderMongoRepository(NorthwindDbContext context) : IOrderMongoRepository
{
    private const string BrokenField = "field14";

    public async Task<IEnumerable<OrderNorthwind>> GetAllAsync()
    {
        var filter = Builders<OrderNorthwind>.Filter.Exists(BrokenField, false);

        return await context.Orders.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<OrderNorthwind>> GetByDateTimeAsync(DateTime? start, DateTime? end)
    {
        var filter = Builders<OrderNorthwind>.Filter.Exists(BrokenField, false);

        var allOrders = await context.Orders.Find(filter).ToListAsync();

        var dateFormat = "yyyy-MM-dd HH:mm:ss.fff";
        var culture = CultureInfo.InvariantCulture;

        var filteredOrders = allOrders
            .Where(o =>
                DateTime.TryParseExact(
                    o.OrderDate,
                    dateFormat,
                    culture,
                    DateTimeStyles.None,
                    out var orderDate) &&
                (!start.HasValue || orderDate >= start.Value) &&
                (!end.HasValue || orderDate <= end.Value))
            .ToList();

        return filteredOrders;
    }

    public async Task<OrderNorthwind?> GetByIdAsync(int id)
    {
        return await context.Orders.AsQueryable().FirstOrDefaultAsync(o => o.OrderID == id);
    }

    public async Task<IEnumerable<OrderNorthwindDetail>> GetOrderDetailsAsync(int id)
    {
        return await context.OrderDetails.AsQueryable().Where(od => od.OrderID == id).ToListAsync();
    }
}
