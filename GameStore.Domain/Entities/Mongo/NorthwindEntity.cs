using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.Entities.Mongo;
public class NorthwindEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
}
