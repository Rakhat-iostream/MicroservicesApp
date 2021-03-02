using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingWeb.ApiCollection.Interfaces;
using ShoppingWeb.Entities;

namespace ShoppingWeb.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly IProductApi _productApi;
        private readonly IBasketApi _basketApi;
        private readonly IWebHostEnvironment _env;
        public ProductDetailModel(IApiFactory factory, IWebHostEnvironment env)
        {
            _productApi = factory.ProductApi ?? throw new ArgumentNullException(nameof(_productApi));
            _basketApi = factory.BasketApi ?? throw new ArgumentNullException(nameof(_basketApi));
            _env = env;
        }

        public Product Product { get; set; }

        [BindProperty]
        public string Color { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        public async Task<IActionResult> OnGetAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return NotFound();
            }
            Product = await _productApi.GetProduct(productId);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId)
        {
            string username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username)) return RedirectToPage("Login", new { loginError = "Please sign in" });
            Product = await _productApi.GetProduct(productId);
            var item = new CartItem
            {
                ProductId = productId,
                Quantity = Quantity,
                Color = Color,
                ProductName = Product.Name,
                Price = Product.Price
            };
            await _basketApi.AddItem(username, item);
            return RedirectToPage("Cart");
        }


        public async Task<IActionResult> OnPostUpdateProductAsync(Product updateProduct, IFormFile file)
        {
            Product product = await _productApi.GetProduct(updateProduct.Id);
            UpdateProductValues(product, updateProduct);
            if (file != null)
            {
                string path = "/images/product/" + file.FileName;
                using (var stream = new FileStream(_env.WebRootPath + path, FileMode.OpenOrCreate))
                {
                    await file.CopyToAsync(stream);
                }
                product.ImageFile = file.FileName;
            }
            if (await _productApi.UpdateProduct(product))
            {
                return RedirectToPage();
            }
            ViewData["updateProductError"] = "Failed to update product";
            return Page();
        }

        private void UpdateProductValues(Product product, Product updateProduct)
        {
            if (!string.IsNullOrEmpty(updateProduct.Name)) product.Name = updateProduct.Name;
            if (!string.IsNullOrEmpty(updateProduct.Summary)) product.Summary = updateProduct.Summary;
            if (!string.IsNullOrEmpty(updateProduct.Description)) product.Description = updateProduct.Description;
            if (updateProduct.Price > 0) product.Price = updateProduct.Price;
            if (!string.IsNullOrEmpty(updateProduct.Category)) product.Category = updateProduct.Category;
        }
    }
}