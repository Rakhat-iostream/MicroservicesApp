using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingWeb.ApiCollection.Interfaces;
using ShoppingWeb.Entities;

namespace ShoppingWeb.Pages
{ 
    public class ProductModel : PageModel
    {
    private readonly IProductApi _productApi;
    private readonly IBasketApi _basketApi;
    public ProductModel(IApiFactory factory)
    {
        _productApi = factory.ProductApi ?? throw new ArgumentNullException(nameof(_productApi));
        _basketApi = factory.BasketApi ?? throw new ArgumentNullException(nameof(_basketApi));
    }

    public IEnumerable<string> CategoryList { get; set; } = new List<string>();
    public IEnumerable<Product> ProductList { get; set; } = new List<Product>();


    [BindProperty(SupportsGet = true)]
    public string SelectedCategory { get; set; }

    public async Task<IActionResult> OnGetAsync(string categoryName, int pageNumber)
    {
        if (!string.IsNullOrEmpty(categoryName))
        {
            ProductList = await _productApi.GetProductByCategory(categoryName);
            SelectedCategory = categoryName;
        }
        else
        {
            ProductList = await _productApi.GetProducts();
            CategoryList = ProductList.Select(p => p.Category).Distinct();
        }
        if (pageNumber > 0)
        {
            ProductList = await _productApi.GetProductByPage(pageNumber);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(string productId)
    {
       /* string username = HttpContext.Session.GetString("username");
        if (string.IsNullOrEmpty(username)) return RedirectToPage("Login", new { loginError = "Please sign in" });*/
        var product = await _productApi.GetProduct(productId);
        var item = new CartItem
        {
            ProductId = product.Id,
            Price = product.Price,
            ProductName = product.Name,
            Quantity = 1
        };
        await _basketApi.AddItem("test", item);
        return RedirectToPage("Cart");
    }

    public async Task<IActionResult> OnPostDeleteProductAsync(string productId)
    {
        if (await _productApi.DeleteProduct(productId))
        {
            return RedirectToPage();
        }
        ViewData["productError"] = "Failed to delete product";
        return Page();
    }
}
}