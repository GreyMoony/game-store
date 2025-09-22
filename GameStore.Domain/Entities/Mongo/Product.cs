using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.Entities.Mongo;
public class Product : NorthwindEntity, ICommonGame
{
    [BsonElement("ProductName")]
    public string Name { get; set; }

    public string Key { get; set; }

    [BsonElement("UnitPrice")]
    public double Price { get; set; }

    [BsonElement("UnitsInStock")]
    public int UnitInStock { get; set; }

    public int ViewCount { get; set; }

    [BsonIgnore]
    public DateTime? CreatedAt { get; set; }

    public int? ProductID { get; set; }

    public int? SupplierID { get; set; }

    public int? CategoryID { get; set; }

    public string? QuantityPerUnit { get; set; }

    public int? UnitsOnOrder { get; set; }

    public int? ReorderLevel { get; set; }

    public bool? Discontinued { get; set; }

    public bool? CopiedToSql { get; set; }

    public bool? IsDeleted { get; set; }

    public int CountComments()
    {
        return 0;
    }
}
