using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace ShoppingWeb.ApiCollection.Infrastructure
{
    public abstract class BaseHttpClientFactory
    {
        private readonly IHttpClientFactory _factory;
        protected HttpRequestBuilder _builder;
        public BaseHttpClientFactory(IHttpClientFactory factory)
        {
            _factory = factory;
        }
        public HttpClient Client { get => _factory.CreateClient(); }

        public virtual async Task<T> GetResponseAsync<T>(HttpRequestMessage request) where T : class
        {
            using var client = Client;
            using var response = await client.SendAsync(request);
            T result = null;
            try
            {
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>(GetFormatters());
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public virtual async Task<string> GetResponseStringAsync(HttpRequestMessage request)
        {
            using var client = Client;
            using var response = await client.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        protected virtual IEnumerable<MediaTypeFormatter> GetFormatters()
        {
            // Make JSON the default
            return new List<MediaTypeFormatter> { new JsonMediaTypeFormatter() };
        }
    }
}
