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
        private string username;

        public CheckOutModel(IApiFactory factory)
        {
            _basketApi = factory.BasketApi ?? throw new ArgumentNullException(nameof(_basketApi));
            _orderApi = factory.OrderApi ?? throw new ArgumentNullException(nameof(_orderApi));
            _usersApi = factory.UsersApi ?? throw new ArgumentNullException(nameof(_usersApi));
        }

        [BindProperty]
        public Order Order { get; set; }

        public Cart Cart { get; set; }

        public User User { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            User = await _usersApi.GetUserById(Guid.Parse(HttpContext.Session.GetString("id")));
            Cart = await _basketApi.GetCart(User.Username);
            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            username = HttpContext.Session.GetString("username");
            Cart = await _basketApi.GetCart(username);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Order.UserName = username;
            Order.TotalPrice = Cart.TotalPrice;

            await _orderApi.Checkout(Order);
            await _basketApi.DeleteCart(Order.UserName);

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}