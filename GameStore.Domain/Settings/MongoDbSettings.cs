namespace GameStore.Domain.Settings;
public class MongoDbSettings
{
    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }

    public string LogsCollection { get; set; }

    public string CategoryCollection { get; set; }

    public string OrderCollection { get; set; }

    public string OrderDetailsCollection { get; set; }

    public string ProductCollection { get; set; }

    public string ShipperCollection { get; set; }

    public string SupplierCollection { get; set; }
}