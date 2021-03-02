using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWeb.ApiCollection.Settings
{
    public interface IApiSettings
    {
        string BaseAddress { get; set; }
        string CatalogPath { get; set; }
        string BasketPath { get; set; }
        string OrderingPath { get; set; }
        string UsersPath { get; set; }
    }
}
