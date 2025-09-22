using GameStore.DAL.Data;
using GameStore.DAL.Repositories.MongoRepositories;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Settings;
using GameStore.Tests.TestUtilities.MongoRepositoriesTestData;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;

namespace GameStore.Tests.TestUtilities;
public class MongoTestFixture : IDisposable
{
    private bool _disposed;

    public MongoTestFixture()
    {
        Runner = MongoDbRunner.Start(singleNodeReplSet: true); // Fast startup
        var client = new MongoClient(Runner.ConnectionString);
        Database = client.GetDatabase("TestDb");

        var options = Options.Create(new MongoDbSettings
        {
            CategoryCollection = "Categories",
            ProductCollection = "Products",
            SupplierCollection = "Suppliers",
            OrderCollection = "Orders",
            OrderDetailsCollection = "OrderDetails",
        });

        Context = new NorthwindDbContext(Database, options);
        CategoryRepository = new CategoryRepository(Context);

        Seed().GetAwaiter().GetResult(); // One-time seed
    }

    ~MongoTestFixture()
    {
        Dispose(false);
    }

    public MongoDbRunner Runner { get; }

    public IMongoDatabase Database { get; }

    public NorthwindDbContext Context { get; }

    public CategoryRepository CategoryRepository { get; }

    public ProductRepository ProductRepository { get; }

    public SupplierRepository SupplierRepository { get; }

    public OrderMongoRepository OrderRepository { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // Fix for CA1816
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Runner?.Dispose();
            }

            _disposed = true;
        }
    }

    private async Task Seed()
    {
        await Context.Categories.DeleteManyAsync(Builders<Category>.Filter.Empty);
        await Context.Products.DeleteManyAsync(Builders<Product>.Filter.Empty);
        await Context.Suppliers.DeleteManyAsync(Builders<Supplier>.Filter.Empty);
        await Context.Orders.DeleteManyAsync(Builders<OrderNorthwind>.Filter.Empty);
        await Context.OrderDetails.DeleteManyAsync(Builders<OrderNorthwindDetail>.Filter.Empty);

        await Context.Categories.InsertManyAsync(MongoDbTestData.Categories);
        await Context.Products.InsertManyAsync(MongoDbTestData.Products);
        await Context.Suppliers.InsertManyAsync(MongoDbTestData.Suppliers);
        await Context.Orders.InsertManyAsync(MongoDbTestData.Orders);
        await Context.OrderDetails.InsertManyAsync(MongoDbTestData.OrderDetails);
    }
}
