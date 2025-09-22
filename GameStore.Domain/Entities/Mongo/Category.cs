using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.Entities.Mongo;

[BsonIgnoreExtraElements]
public class Category : NorthwindEntity
{
    public int CategoryID { get; set; }

    public string CategoryName { get; set; }

    public string Description { get; set; }

    public string Picture { get; set; }

    public bool? CopiedToSql { get; set; }

    public bool? IsDeleted { get; set; }
}
