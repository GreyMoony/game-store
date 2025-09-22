using GameStore.DAL.Interfaces;

namespace GameStore.DAL.Repositories.MongoRepositories;

#pragma warning disable S3604
public class UnitOfWorkNorthwind(
    IProductRepository products,
    ICategoryRepository categories,
    ISupplierRepository suppliers,
    IOrderMongoRepository orders,
    IShipperRepository shippers) : IUnitOfWorkMongo
{
    public IProductRepository Products { get; } = products;

    public ICategoryRepository Categories { get; } = categories;

    public ISupplierRepository Suppliers { get; } = suppliers;

    public IOrderMongoRepository Orders { get; } = orders;

    public IShipperRepository Shippers { get; } = shippers;
}
