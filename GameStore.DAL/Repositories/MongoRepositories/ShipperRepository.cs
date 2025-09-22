using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Constants;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStore.DAL.Repositories.MongoRepositories;
public class ShipperRepository(NorthwindDbContext context) : IShipperRepository
{
    public async Task<IEnumerable<Dictionary<string, object>>> GetAll()
    {
        var bsonDocuments = await context
            .GetCollection<BsonDocument>(NorthwindNames.ShipperCollection)
            .Find(new BsonDocument()).ToListAsync();

        // Convert BsonDocument to Dictionary<string, object>
        return bsonDocuments.Select(doc => doc.ToDictionary());
    }
}
