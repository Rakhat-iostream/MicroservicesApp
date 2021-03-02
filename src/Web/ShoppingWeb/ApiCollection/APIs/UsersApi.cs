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
    public class UsersApi : BaseHttpClientFactory, IUsersApi
    {
        private readonly IApiSettings _settings;

        public UsersApi(IApiSettings settings, IHttpClientFactory factory) : base(factory)
        {
            _settings = settings;
            _builder = new HttpRequestBuilder(_settings.BaseAddress);
            _builder.AddToPath(_settings.UsersPath);
        }

        public async Task<User> GetUserById(Guid id)
        {
            using var message = _builder
             .HttpMethod(HttpMethod.Get).AddToPath(id.ToString())
             .GetHttpMessage();
            return await GetResponseAsync<User>(message);
        }

        public async Task<string> AuthentificationToken(User user)
        {
            using var message = _builder
            .HttpMethod(HttpMethod.Post).AddToPath("/login").
            Content(new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"))
            .GetHttpMessage();
            return await GetResponseStringAsync(message);
        }

        public async Task<string> RegistrationToken(User user)
        {
            using var message = _builder
            .HttpMethod(HttpMethod.Post).AddToPath("/register").
            Content(new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"))
            .GetHttpMessage();
            return await GetResponseStringAsync(message);
        }

        public async Task<bool> UpdateUser(User user)
        {
            if (_builder != null) _builder.Dispose();
            using (_builder = new HttpRequestBuilder(_settings.BaseAddress))
            {
                _builder.AddToPath(_settings.UsersPath);
                using var message = _builder
            .HttpMethod(HttpMethod.Put).AddToPath("/" + user.Id).
            Content(new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"))
            .GetHttpMessage();
                return await GetResponseStringAsync(message) != null;
            }
        }

        public override async Task<string> GetResponseStringAsync(HttpRequestMessage request)
        {
            using var client = Client;
            using var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
