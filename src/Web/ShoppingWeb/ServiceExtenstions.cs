using Microsoft.Extensions.DependencyInjection;
using ShoppingWeb.ApiCollection.APIs;
using ShoppingWeb.ApiCollection.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWeb
{
    public static class ServicesExtensions
    {
        public static void AddAPIs(this IServiceCollection services)
        {
            services.AddScoped<IBasketApi, BasketApi>();
            services.AddScoped<IOrderApi, OrderApi>();
            services.AddScoped<IProductApi, ProductApi>();
            services.AddScoped<IUsersApi, UsersApi>();

            services.AddScoped<IApiFactory, ApiFactory>();
        }
    }
}
