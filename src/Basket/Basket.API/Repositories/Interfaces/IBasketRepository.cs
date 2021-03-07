using Basket.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Repositories.Interfaces
{
    public interface IBasketRepository
    {
        Task<BasketCart> GetBasket(string username);
        Task<BasketCart> AddBasketItem(string username, BasketCartItem basketCartItem);
        Task<BasketCart> UpdateBasket(BasketCart cart);
        Task<bool> DeleteBasket(string username);
        Task<bool> DeleteBasketItem(string username, BasketCartItem targetItem);
        Task<bool> UpdateBasketItems(BasketCart basket);
    }
}
