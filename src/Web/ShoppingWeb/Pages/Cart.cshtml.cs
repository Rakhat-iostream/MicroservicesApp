using System;
using System.Threading.Tasks;
using ShoppingWeb.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingWeb.ApiCollection.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ShoppingWeb.Pages
{
    public class CartModel : PageModel
    {
        private readonly IBasketApi _api;
        public CartModel(IApiFactory factory)
        {
            _api = factory.BasketApi ?? throw new ArgumentNullException(nameof(_api));
        }

        public Cart Cart { get; set; } = new Cart();

        public async Task<IActionResult> OnGetAsync()
        {
           /* username = HttpContext.Session.GetString("username");*/
            Cart = await _api.GetCart("test");
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(string cartItemId)
        {
            /*username = HttpContext.Session.GetString("username");*/
            Cart = await _api.GetCart("test");
            await _api.DeleteItem(Cart.Username, Cart.Items.Find(i => i.ProductId == cartItemId));
            return RedirectToPage();
        }
    }
}