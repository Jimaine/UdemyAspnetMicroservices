using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Respositories
{
    public class ProductRespository : IProductRespository
    {
        private readonly ICatalogContext _catalogContext;

        public ProductRespository(ICatalogContext catalogContext)
        {
            _catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
        }

        public async Task CreateProduct(Product productToInsert)
        {
            await _catalogContext.Products.InsertOneAsync(productToInsert);
        }

        public async Task<bool> DeleteProduct(string productId)
        {
            FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.Eq(product => product.Id, productId);
            var deleteResult = await _catalogContext.Products.DeleteOneAsync(filterDefinition);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<Product> GetProduct(string productId)
        {
            return await _catalogContext.Products.Find(product => product.Id == productId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
        {
            FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.Eq(product => product.Category, categoryName);

            return await _catalogContext.Products.Find(filterDefinition).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string productName)
        {
            FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.Eq(product => product.Name, productName);

            return await _catalogContext.Products.Find(filterDefinition).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _catalogContext.Products.Find(product => true).ToListAsync();
        }

        public async Task<bool> UpdateProduct(Product productToUpdate)
        {
            var updateResult = await _catalogContext.Products.ReplaceOneAsync(filter: product => product.Id == productToUpdate.Id, replacement: productToUpdate);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}
