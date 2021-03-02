using Newtonsoft.Json;
using ShoppingWeb.ApiCollection.Infrastructure;
using ShoppingWeb.ApiCollection.Interfaces;
using ShoppingWeb.ApiCollection.Settings;
using ShoppingWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingWeb.ApiCollection.APIs
{
    public class OrderApi : BaseHttpClientFactory, IOrderApi
    {
        private IApiSettings _settings;
        public OrderApi(IHttpClientFactory factory, IApiSettings settings) : base(factory)
        {
            _settings = settings;
            _builder = new HttpRequestBuilder(_settings.BaseAddress);
            _builder.SetPath(_settings.OrderingPath);
        }

        public async Task Checkout(Order order)
        {
            _builder.SetPath(_settings.BasketPath).AddToPath("/Checkout");
            using var message = _builder.Content(new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json"))
            .HttpMethod(HttpMethod.Post)
            .GetHttpMessage();
            await GetResponseStringAsync(message);
        }

        public async Task<bool> DeleteOrderById(int id)
        {
            using var message = _builder.AddToPath("/" + id)
            .HttpMethod(HttpMethod.Delete)
            .GetHttpMessage();
            return await GetResponseStringAsync(message) != null;
        }

        public async Task<Order> GetOrderById(int id)
        {
            using var message = _builder.AddToPath("/" + id)
            .HttpMethod(HttpMethod.Get)
            .GetHttpMessage();
            return await GetResponseAsync<Order>(message);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUsername(string username)
        {
            using var message = _builder.AddQueryString("username", username)
            .HttpMethod(HttpMethod.Get)
            .GetHttpMessage();
            return await GetResponseAsync<IEnumerable<Order>>(message);
        }
    }
}
