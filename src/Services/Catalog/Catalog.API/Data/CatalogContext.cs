using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public IMongoCollection<Product> Products { get; }

        public CatalogContext(IConfiguration configuration)
        {
            string connectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString");
            string databaseName = configuration.GetValue<string>("DatabaseSettings:DatabaseName");
            string collectionName = configuration.GetValue<string>("DatabaseSettings:CollectionName");

            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase(databaseName);

            Products = database.GetCollection<Product>(collectionName);
            CatalogContextSeed.SeedData(Products);
        }
    }
}
