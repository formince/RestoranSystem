using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.Core.Business;
using Restoran.Core.DTOs.Card;
using Restoran.Core.DTOs.Order;
using System.Security.Claims;
using System.Text.Json;

namespace Restoran.Web.Controllers
{
    public class CartController : Controller
    {
        private const string CartCookieKey = "cart";

        private List<CartItemDto> GetCartFromCookie()
        {
            var cartJson = Request.Cookies[CartCookieKey];
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItemDto>()
                : JsonSerializer.Deserialize<List<CartItemDto>>(cartJson);
        }

        private void SaveCartToCookie(List<CartItemDto> cart)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                HttpOnly = false
            };

            var json = JsonSerializer.Serialize(cart);
            Response.Cookies.Append(CartCookieKey, json, options);
        }

        public async Task<IActionResult> Cart()
        {
            var cart = GetCartFromCookie();

            if (!cart.Any())
            {
                return View(new List<CartItemViewModel>());
            }

            // Ürün detaylarını getir
            var productIds = cart.Select(c => c.ProductId).ToList();
            var products = await new BLLProduct().GetProductsByIdsAsync(productIds);

            var viewModel = cart.Select(ci =>
            {
                var product = products.FirstOrDefault(x => x.Id == ci.ProductId);
                return new CartItemViewModel
                {
                    ProductId = ci.ProductId,
                    ProductName = product?.Name ?? "Ürün bulunamadı",
                    Quantity = ci.Quantity,
                    UnitPrice = product?.Price ?? 0
                };
            }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var cart = GetCartFromCookie();

            var existing = cart.FirstOrDefault(x => x.ProductId == productId);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItemDto
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            SaveCartToCookie(cart);
            TempData["Message"] = "Ürün sepete eklendi.";

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartFromCookie();
            cart.RemoveAll(x => x.ProductId == productId);
            SaveCartToCookie(cart);

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCartFromCookie();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
            SaveCartToCookie(cart);
            return RedirectToAction("Cart");
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCartFromCookie();
            if (cart == null || !cart.Any())
            {
                TempData["Message"] = "Sepetiniz boş.";
                return RedirectToAction("Cart");
            }

            // Kullanıcı ID'sini claims'den al
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                TempData["Message"] = "Kullanıcı bilgisi bulunamadı.";
                return RedirectToAction("Login", "Account");
            }

            var orderDto = new OrderCreateDto
            {
                UserId = userId,
                Items = cart.Select(x => new OrderItemDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList()
            };

            var bllOrder = new BLLOrder();
            await bllOrder.CreateOrderAsync(orderDto);

            Response.Cookies.Delete(CartCookieKey);
            TempData["Message"] = "Sipariş başarıyla oluşturuldu.";

            return RedirectToAction("Cart");
        }
    }
}
