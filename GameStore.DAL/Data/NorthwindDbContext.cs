using GameStore.Domain.Constants;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.DAL.Data;
public class NorthwindDbContext(IMongoDatabase database, IOptions<MongoDbSettings> options)
{
    private readonly string _categoryCollectionName = options.Value.CategoryCollection
        ?? NorthwindNames.CategoryCollection;

    private readonly string _orderCollectionName = options.Value.OrderCollection
        ?? NorthwindNames.OrderCollection;

    private readonly string _orderDetailCollectionName = options.Value.OrderDetailsCollection
        ?? NorthwindNames.OrderDetailCollection;

    private readonly string _productCollectionName = options.Value.ProductCollection
        ?? NorthwindNames.ProductCollection;

    private readonly string _supplierCollectionName = options.Value.SupplierCollection
        ?? NorthwindNames.SupplierCollection;

    public IMongoCollection<Product> Products =>
        database.GetCollection<Product>(_productCollectionName);

    public IMongoCollection<Category> Categories =>
        database.GetCollection<Category>(_categoryCollectionName);

    public IMongoCollection<Supplier> Suppliers =>
        database.GetCollection<Supplier>(_supplierCollectionName);

    public IMongoCollection<OrderNorthwind> Orders =>
        database.GetCollection<OrderNorthwind>(_orderCollectionName);

    public IMongoCollection<OrderNorthwindDetail> OrderDetails =>
        database.GetCollection<OrderNorthwindDetail>(_orderDetailCollectionName);

    public IMongoCollection<T> GetCollection<T>(string name) =>
        database.GetCollection<T>(name);
}