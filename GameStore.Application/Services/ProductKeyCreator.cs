using GameStore.Application.Helpers;
using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities.Mongo;
using MongoDB.Driver;

namespace GameStore.Application.Services;
public class ProductKeyCreator(NorthwindDbContext mongoContext, IGameRepository gameRepository)
{
    public async Task AddKeyToProductsAsync()
    {
        var productsCollection = mongoContext.GetCollection<Product>("products");

        // Fetch all existing keys from MongoDB
        var existingMongoKeys = await productsCollection
            .Find(Builders<Product>.Filter.Exists("Key"))
            .Project(p => p.Key)
            .ToListAsync();

        // Fetch all existing keys from SQL Server
        var existingSqlKeys = await gameRepository.GetAllKeysAsync();

        // Combine all existing keys to ensure uniqueness
        var allExistingKeys = new HashSet<string>(existingMongoKeys.Concat(existingSqlKeys));

        // Find products without the "Key" field
        var filter = Builders<Product>.Filter.Exists("Key", false);
        var productsWithoutKey = await productsCollection.Find(filter).ToListAsync();

        foreach (var product in productsWithoutKey)
        {
            // Generate a unique key
            string baseKey = KeyStringHelper.CreateKey(product.Name);
            string uniqueKey = baseKey;
            int suffix = 1;
            while (allExistingKeys.Contains(uniqueKey))
            {
                uniqueKey = $"{baseKey}-{suffix}";
                suffix++;
            }

            // Update the document with the new key
            var update = Builders<Product>.Update.Set("Key", uniqueKey);
            await productsCollection.UpdateOneAsync(
                Builders<Product>.Filter.Eq("_id", product.Id),
                update);

            // Add the new key to the set to avoid duplicates
            allExistingKeys.Add(uniqueKey);
        }
    }
}
