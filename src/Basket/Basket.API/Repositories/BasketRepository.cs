using Basket.API.Data.Interfaces;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IBasketContext _context;

        public BasketRepository(IBasketContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<BasketCart> AddBasketItem(string username, BasketCartItem basketCartItem)
        {
            var basketFromRedis = await _context.Redis.StringGetAsync(username);
            if (basketFromRedis.IsNullOrEmpty)
            {
                await UpdateBasketItems(new BasketCart { Username = username });
                basketFromRedis = await _context.Redis.StringGetAsync(username);
            }

            try
            {
                var basket = JsonConvert.DeserializeObject<BasketCart>(basketFromRedis);
                basket.Items.Add(basketCartItem);
                if (await UpdateBasketItems(basket))
                {
                    return basket;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteBasket(string username)
        {
            return await _context.Redis.KeyDeleteAsync(username);
        }

        public async Task<bool> DeleteBasketItem(string username, BasketCartItem targetItem)
        {
            var basketFromRedis = await _context.Redis.StringGetAsync(username);
            if (basketFromRedis.IsNullOrEmpty) return false;

            try
            {
                var basket = JsonConvert.DeserializeObject<BasketCart>(basketFromRedis);
                basket.Items.RemoveAll(item => item.ProductId.Equals(targetItem.ProductId));
                return await UpdateBasketItems(basket);
            }
            catch
            {
                return false;
            }
        }

        public async Task<BasketCart> GetBasket(string username)
        {
            var basket = await _context.Redis.StringGetAsync(username);

            if (basket.IsNullOrEmpty)
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<BasketCart>(basket);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<BasketCart> UpdateBasket(BasketCart cart)
        {
            var isUpdated = await _context.Redis.StringSetAsync(cart.Username, JsonConvert.SerializeObject(cart));
            if (!isUpdated)
            {
                return null;
            }

            return await GetBasket(cart.Username);
        }

        public async Task<bool> UpdateBasketItems(BasketCart basket)
        {
            return await _context.Redis.StringSetAsync(basket.Username, JsonConvert.SerializeObject(basket));
        }
    }
}