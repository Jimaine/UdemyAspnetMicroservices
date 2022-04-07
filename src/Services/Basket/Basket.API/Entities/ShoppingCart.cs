using System.Collections.Generic;

namespace Basket.API.Entities
{
    public class ShoppingCart
    {
        public string UserName { get; set; }
        public List<ShoppingcartItem> Items { get; set; } = new List<ShoppingcartItem>();
        public decimal TotalPrice 
        { 
            get 
            {
                decimal totalPrice = 0;

                foreach (var item in Items)
                {
                    totalPrice += item.Price * item.Quantity;
                }

                return totalPrice;
            } 
        }

        public ShoppingCart()
        {

        }

        public ShoppingCart(string userName)
        {
            UserName = userName;
        }
    }
}
