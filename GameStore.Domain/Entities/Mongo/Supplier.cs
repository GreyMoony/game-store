using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.Entities.Mongo;

[BsonIgnoreExtraElements]
public class Supplier : NorthwindEntity
{
    public int SupplierID { get; set; }

    public string CompanyName { get; set; }

    public string ContactName { get; set; }

    public string ContactTitle { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string Region { get; set; }

    public object PostalCode { get; set; }

    public string Country { get; set; }

    public object Phone { get; set; }

    public object Fax { get; set; }

    public string HomePage { get; set; }

    public bool? CopiedToSql { get; set; }

    public bool? IsDeleted { get; set; }
}
