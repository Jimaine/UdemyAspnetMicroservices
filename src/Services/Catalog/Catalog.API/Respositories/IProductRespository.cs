using Catalog.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Respositories
{
    public interface IProductRespository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProduct(string productId);
        Task<IEnumerable<Product>> GetProductsByName(string productName);
        Task<IEnumerable<Product>> GetProductsByCategory(string categoryName);
        Task CreateProduct(Product productToInsert);
        Task<bool> UpdateProduct(Product productToUpdate);
        Task<bool> DeleteProduct(string productId);
    }
}
