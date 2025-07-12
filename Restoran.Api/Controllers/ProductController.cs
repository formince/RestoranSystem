using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.Core.Business;
using Restoran.Core.DTOs.Product;

namespace Restoran.Api.Controllers
{
    public class ProductController : BaseController
    {

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts()
        {
            var productBLL = new BLLProduct(WebRootPath);
            var products = await productBLL.GetProductsAsync();
            return HandleResult(products, "Ürünler başarıyla getirildi");
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id)
        {
            var productBLL = new BLLProduct(WebRootPath);
            var product = await productBLL.GetProductByIdAsync(id);
            return HandleResult(product, "Ürün başarıyla getirildi");
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto dto, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            byte[]? imageData = null;
            string? imageFileName = null;

            // Resim var mı kontrol et
            if (image != null && image.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
                imageFileName = image.FileName;
            }

            var productBLL = new BLLProduct(WebRootPath);
            var result = await productBLL.CreateProductAsync(dto, imageData, imageFileName);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDto dto, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            byte[]? imageData = null;
            string? imageFileName = null;

            // Resim var mı kontrol et
            if (image != null && image.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
                imageFileName = image.FileName;
            }

            var productBLL = new BLLProduct(WebRootPath);
            var result = await productBLL.UpdateProductAsync(id, dto, imageData, imageFileName);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productBLL = new BLLProduct(WebRootPath);
            var result = await productBLL.DeleteProductAsync(id);
            return HandleResult(result);
        }
    }
} 