using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace GameStore.Tests.TestUtilities;
public static class MongoDefinitionExtensions
{
    public static BsonDocument RenderToBsonDocument<T>(this FilterDefinition<T> filter)
    {
        var args = new RenderArgs<T>(BsonSerializer.SerializerRegistry.GetSerializer<T>(), BsonSerializer.SerializerRegistry);

        var bsonDocument = filter.Render(args);
        Console.WriteLine("Rendered Filter: " + bsonDocument.ToJson());

        return bsonDocument;
    }

    public static BsonDocument RenderToBsonDocument<T>(this UpdateDefinition<T> update)
    {
        var args = new RenderArgs<T>(BsonSerializer.SerializerRegistry.GetSerializer<T>(), BsonSerializer.SerializerRegistry);

        var renderedValue = update.Render(args);

        return renderedValue.IsBsonDocument ? renderedValue.AsBsonDocument : new BsonDocument("value", renderedValue);
    }
}
