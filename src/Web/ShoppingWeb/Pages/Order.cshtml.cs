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
    public class OrderModel : PageModel
    {
        private readonly IOrderApi _orderApi;
        private string username;

        public OrderModel(IApiFactory factory)
        {
            _orderApi = factory.OrderApi ?? throw new ArgumentNullException(nameof(_orderApi));
        }

        public IEnumerable<Order> Orders { get; set; } = new List<Order>();

        public async Task<IActionResult> OnGetAsync()
        {
            username = HttpContext.Session.GetString("username");
            Orders = await _orderApi.GetOrdersByUsername(username);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteOrderByIdAsync(int orderId)
        {
            if (await _orderApi.DeleteOrderById(orderId))
            {
                return RedirectToPage();
            }
            return RedirectToPage(new { orderDeleteError = "Failed to delete order" });
        }
    }
}