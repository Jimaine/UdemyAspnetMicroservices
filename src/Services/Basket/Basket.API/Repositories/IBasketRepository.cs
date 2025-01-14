﻿using Basket.API.Entities;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetBasket(string userName);
        Task<ShoppingCart> SetBasket(ShoppingCart shoppingCart);
        Task DeleteBasket(string userName);

    }
}
