namespace GameStore.DAL.Interfaces;
public interface IUnitOfWorkMongo
{
    IProductRepository Products { get; }

    ICategoryRepository Categories { get; }

    ISupplierRepository Suppliers { get; }

    IOrderMongoRepository Orders { get; }

    IShipperRepository Shippers { get; }
}
