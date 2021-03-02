using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWeb.Entities
{
    public class Cart
    {
        public string Username { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public Cart()
        {
        }

        public Cart(string username)
        {
            Username = username;
        }

        public decimal TotalPrice
        {
            get
            {
                decimal totalprice = 0;
                foreach (var item in Items)
                {
                    totalprice += item.Price * item.Quantity;
                }

                return totalprice;
            }
        }
    }
}
