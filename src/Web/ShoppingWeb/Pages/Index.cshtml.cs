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
        private string username;

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

        public async Task<IActionResult> OnPostAddToCartAsync(CartItem cartItem)
        {
            //if (!User.Identity.IsAuthenticated)
            //    return RedirectToPage("./Account/Login", new { area = "Identity" });
            await _basketApi.AddItem("test", cartItem);
            return RedirectToPage("Cart");
        }
    }
}
