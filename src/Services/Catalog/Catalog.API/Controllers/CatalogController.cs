using Catalog.API.Entities;
using Catalog.API.Respositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRespository _productRespository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRespository productRespository, ILogger<CatalogController> logger)
        {
            _productRespository = productRespository ?? throw new ArgumentNullException(nameof(productRespository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productRespository.GetProducts();

            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProduct(string productId)
        {
            var product = await _productRespository.GetProduct(productId);

            if (product is null)
            {
                _logger.LogError($"Product with id: {productId}, not found.");
                return NotFound();
            }

            return Ok(product);
        }

        [Route("[action]/{category}", Name = "GetProductsByCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string categoryName)
        {
            var products = await _productRespository.GetProductsByCategory(categoryName);

            return Ok(products);
        }

        [Route("[action]/{category}", Name = "GetProductsByName")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByName(string prodcutName)
        {
            var products = await _productRespository.GetProductsByName(prodcutName);

            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProduct([FromBody] Product productToInsert)
        {
            await _productRespository.CreateProduct(productToInsert);

            return CreatedAtRoute("GetProduct", new { id = productToInsert.Id }, productToInsert);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product productToUpdate)
        {
            return Ok(await _productRespository.UpdateProduct(productToUpdate));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            return Ok(await _productRespository.DeleteProduct(productId));
        }
    }
}
