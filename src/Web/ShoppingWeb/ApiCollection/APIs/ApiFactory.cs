using ShoppingWeb.ApiCollection.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWeb.ApiCollection.APIs
{
    public class ApiFactory : IApiFactory
    {
        private IBasketApi _basketApi;
        private IOrderApi _orderApi;
        private IUsersApi _usersApi;
        private IProductApi _productApi;

        public ApiFactory(IBasketApi basketApi, IOrderApi orderApi, IUsersApi usersApi, IProductApi productApi)
        {
            _basketApi = basketApi;
            _orderApi = orderApi;
            _usersApi = usersApi;
            _productApi = productApi;
        }

        public IBasketApi BasketApi
        {
            get => _basketApi;
        }

        public IUsersApi UsersApi => _usersApi;

        public IOrderApi OrderApi => _orderApi;

        public IProductApi ProductApi => _productApi;
    }
}
