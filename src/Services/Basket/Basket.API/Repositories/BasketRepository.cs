using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            string shoppingCart = await _redisCache.GetStringAsync(userName);

            if (string.IsNullOrWhiteSpace(shoppingCart))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ShoppingCart>(shoppingCart);
        }

        public async Task<ShoppingCart> SetBasket(ShoppingCart shoppingCart)
        {
            await _redisCache.SetStringAsync(shoppingCart.UserName, JsonConvert.SerializeObject(shoppingCart));

            return await GetBasket(shoppingCart.UserName);
        }

        public async Task DeleteBasket(string userName)
        {
            await _redisCache.RemoveAsync(userName);
        }
    }
}
