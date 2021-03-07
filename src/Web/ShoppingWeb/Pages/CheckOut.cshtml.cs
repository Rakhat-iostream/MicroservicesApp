using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingWeb.ApiCollection.Interfaces;
using ShoppingWeb.Entities;

namespace ShoppingWeb.Pages
{
    public class CheckOutModel : PageModel
    {
        private readonly IBasketApi _basketApi;
        private readonly IOrderApi _orderApi;
        private readonly IUsersApi _usersApi;

        public CheckOutModel(IApiFactory factory)
        {
            _basketApi = factory.BasketApi ?? throw new ArgumentNullException(nameof(_basketApi));
            _orderApi = factory.OrderApi ?? throw new ArgumentNullException(nameof(_orderApi));
            _usersApi = factory.UsersApi ?? throw new ArgumentNullException(nameof(_usersApi));
        }

        [BindProperty]
        public Order Order { get; set; }

        public Cart Cart { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Cart = await _basketApi.GetCart("test");
            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
           /* username = HttpContext.Session.GetString("username");*/
            Cart = await _basketApi.GetCart("test");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Order.UserName = "test";
            Order.TotalPrice = Cart.TotalPrice;

            await _orderApi.Checkout(Order);
            await _basketApi.DeleteCart(Order.UserName);

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}