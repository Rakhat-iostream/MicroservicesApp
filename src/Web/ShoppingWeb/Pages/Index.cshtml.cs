using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingWeb.ApiCollection.Interfaces;
using ShoppingWeb.Entities;

namespace ShoppingWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IProductApi _productApi;
        private readonly IBasketApi _basketApi;

        public IndexModel(IApiFactory factory)
        {
            _productApi = factory.ProductApi ?? throw new ArgumentNullException(nameof(_productApi));
            _basketApi = factory.BasketApi ?? throw new ArgumentNullException(nameof(_basketApi));
        }

        public IEnumerable<Product> ProductList { get; set; } = new List<Product>();

        public async Task<IActionResult> OnGetAsync()
        {
            ProductList = await _productApi.GetProducts();
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId)
        {
            //if (!User.Identity.IsAuthenticated)
            //    return RedirectToPage("./Account/Login", new { area = "Identity" });
            var product = await _productApi.GetProduct(productId);

            var item = new CartItem() { ProductId = product.Id, ProductName = product.Name, Quantity = 1, Color = "Red", Price = product.Price };

            await _basketApi.AddItem("test", item);
            return RedirectToPage("Cart");
        }
    }
}
