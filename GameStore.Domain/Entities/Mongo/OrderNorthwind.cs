using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.Entities.Mongo;

[BsonIgnoreExtraElements]
public class OrderNorthwind : NorthwindEntity
{
    public int OrderID { get; set; }

    public string CustomerID { get; set; }

    public int EmployeeID { get; set; }

    public string OrderDate { get; set; }

    public string RequiredDate { get; set; }

    public string ShippedDate { get; set; }

    public int ShipVia { get; set; }

    public double Freight { get; set; }

    public string ShipName { get; set; }

    public string ShipAddress { get; set; }

    public string ShipCity { get; set; }

    public string ShipRegion { get; set; }

    public object ShipPostalCode { get; set; }

    public string ShipCountry { get; set; }
}
